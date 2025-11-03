using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;


namespace MoodleToGoogleClassroomSync.Utils {
    /// <summary>
    /// Generates a human-readable hierarchical structure of namespaces,
    /// classes, and their members (fields, properties, methods).
    /// </summary>
    public class StructureGenerator {

        /// <summary>
        /// Generates a text-based class and namespace structure for the given namespaces.
        /// </summary>
        /// <param name="namespaces">Array of root namespaces to scan.</param>
        /// <returns>Formatted string representing the structure tree.</returns>
        public static string GenerateStructure(string[] namespaces) {
            if(namespaces == null || namespaces.Length == 0)
                throw new ArgumentException("At least one namespace must be provided.", nameof(namespaces));

            var sb = new StringBuilder();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach(var rootNs in namespaces.OrderBy(n => n)) {
                sb.AppendLine($"Namespace: {rootNs}");

                // Get all types that start with the namespace (including sub-namespaces)
                var typesInNs = assemblies
                    .SelectMany(a => SafeGetTypes(a))
                    .Where(t => t.Namespace != null && t.Namespace.StartsWith(rootNs))
                    .OrderBy(t => t.Namespace)
                    .ThenBy(t => t.Name)
                    .ToList();

                if(!typesInNs.Any()) {
                    sb.AppendLine("  (no types found)");
                    continue;
                }

                // Group by namespace
                var groupedByNs = typesInNs.GroupBy(t => t.Namespace);
                foreach(var nsGroup in groupedByNs) {
                    var indentLevel = CountNamespaceDepth(nsGroup.Key, rootNs);
                    sb.AppendLine($"{new string(' ', indentLevel * 2)}📦 Namespace: {nsGroup.Key}");

                    foreach(var type in nsGroup.OrderBy(t => t.Name)) {
                        if(!type.IsClass && !type.IsValueType)
                            continue;

                        // Skip compiler-generated async/iterator state machines and nested helpers
                        if(type.Name.StartsWith("<") || type.Name.Contains(">"))
                            continue;
                        if(type.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
                            continue;
                        if(type.IsNestedPrivate)
                            continue;
                        if(type.Name.Contains("AnonymousType"))
                            continue;


                        // Class or struct
                        string typeKind = type.IsClass ? "class" : "struct";
                        sb.AppendLine($"{new string(' ', (indentLevel + 1) * 2)}📘 {typeKind} {type.Name}");

                        // --- Static properties/fields ---
                        var staticMembers = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                .Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                                                .ToList();
                        foreach(var sm in staticMembers)
                            sb.AppendLine($"{new string(' ', (indentLevel + 2) * 2)}⚙️ static {GetMemberSignature(sm)}");

                        // --- Public instance properties/fields ---
                        var instanceMembers = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                  .Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                                                  .ToList();
                        foreach(var im in instanceMembers)
                            sb.AppendLine($"{new string(' ', (indentLevel + 2) * 2)}🔸 {GetMemberSignature(im)}");

                        // --- Public instance methods ---
                        var publicMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                .Where(m => !m.IsSpecialName);
                        foreach(var method in publicMethods)
                            sb.AppendLine($"{new string(' ', (indentLevel + 2) * 2)}🔹 method {GetMethodSignature(method)}");

                        // --- Public static methods ---
                        var staticMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                .Where(m => !m.IsSpecialName);
                        foreach(var method in staticMethods)
                            sb.AppendLine($"{new string(' ', (indentLevel + 2) * 2)}⚡ static method {GetMethodSignature(method)}");
                    }
                }


                sb.AppendLine();
            }

            return sb.ToString();
        }

        // === Helper methods ===

        private static IEnumerable<Type> SafeGetTypes(Assembly a) {
            try {
                return a.GetTypes();
            }
            catch(ReflectionTypeLoadException ex) {
                return ex.Types.Where(t => t != null)!;
            }
            catch {
                return Enumerable.Empty<Type>();
            }
        }

        private static int CountNamespaceDepth(string fullNamespace, string rootNamespace) {
            if(string.Equals(fullNamespace, rootNamespace, StringComparison.Ordinal))
                return 1;

            var relative = fullNamespace.Substring(rootNamespace.Length).Trim('.');
            return 1 + relative.Count(c => c == '.');
        }

        private static string GetFriendlyTypeName(Type type) {
            if(type == null)
                return "unknown";

            // Handle nullable value types like DateTime?
            if(Nullable.GetUnderlyingType(type) is Type underlying)
                return GetFriendlyTypeName(underlying) + "?";

            // Handle generic types (e.g., List<int>, Dictionary<string, object>)
            if(type.IsGenericType) {
                var name = type.Name;
                var tickIndex = name.IndexOf('`');
                if(tickIndex >= 0)
                    name = name.Substring(0, tickIndex);

                var args = type.GetGenericArguments()
                               .Select(GetFriendlyTypeName)
                               .ToArray();
                return $"{name}<{string.Join(", ", args)}>";
            }

            // Use C# aliases where possible
            return type switch {
                _ when type == typeof(string) => "string",
                _ when type == typeof(int) => "int",
                _ when type == typeof(bool) => "bool",
                _ when type == typeof(double) => "double",
                _ when type == typeof(float) => "float",
                _ when type == typeof(decimal) => "decimal",
                _ when type == typeof(void) => "void",
                _ when type == typeof(object) => "object",
                _ when type == typeof(byte) => "byte",
                _ when type == typeof(char) => "char",
                _ => type.Name
            };
        }

        private static string GetMemberSignature(MemberInfo member) {
            switch(member) {
                case PropertyInfo prop:
                    return $"{GetFriendlyTypeName(prop.PropertyType)} {prop.Name}";
                case FieldInfo field:
                    return $"{GetFriendlyTypeName(field.FieldType)} {field.Name}";
                default:
                    return member.Name;
            }
        }

        private static string GetMethodSignature(MethodInfo method) {
            var parameters = method.GetParameters();
            var paramList = string.Join(", ", parameters.Select(p => $"{GetFriendlyTypeName(p.ParameterType)} {p.Name}"));
            return $"{GetFriendlyTypeName(method.ReturnType)} {method.Name}({paramList})";
        }

    }
}
