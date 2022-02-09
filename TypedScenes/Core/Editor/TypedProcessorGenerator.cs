#if UNITY_EDITOR
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public static class TypedProcessorGenerator
    {
        public static string Generate(Scene scene)
        {
            var sceneName = scene.name;
            var targetUnit = new CodeCompileUnit();
            var targetNamespace = new CodeNamespace(TypedSceneSettings.Namespace);
            var targetClass = new CodeTypeDeclaration(sceneName + TypedSceneSettings.TypedProcessor);
            targetClass.BaseTypes.Add(TypedSceneSettings.TypedProcessor);
            targetClass.TypeAttributes = System.Reflection.TypeAttributes.Class | System.Reflection.TypeAttributes.Public;

            AddConstantValue(targetClass, typeof(string), "_sceneName", sceneName);
            AddProperty(targetClass, typeof(string), "SceneName", "_sceneName");

            targetNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(targetNamespace);

            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            var code = new StringWriter();
            provider.GenerateCodeFromCompileUnit(targetUnit, code, options);

            return code.ToString();
        }

        private static void AddConstantValue(CodeTypeDeclaration targetClass, Type type, string name, string value)
        {
            var pathConstant = new CodeMemberField(type, name);
            pathConstant.Attributes = MemberAttributes.Private | MemberAttributes.Const;
            pathConstant.InitExpression = new CodePrimitiveExpression(value);
            targetClass.Members.Add(pathConstant);
        }

        private static void AddProperty(CodeTypeDeclaration targetClass, Type type, string name, string value)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Name = name;
            property.Type = new CodeTypeReference(type);
            property.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(value)));
            targetClass.Members.Add(property);
        }
    }
}
#endif
