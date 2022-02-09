#if UNITY_EDITOR
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public static class TypedSceneGenerator
    {
        public static string Generate(AnalyzableScene scene)
        {
            var sceneName = scene.Name;
            var targetUnit = new CodeCompileUnit();
            var targetNamespace = new CodeNamespace(TypedSceneSettings.Namespace);
            var targetClass = new CodeTypeDeclaration(sceneName);
            targetNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine.SceneManagement"));
            targetClass.BaseTypes.Add("TypedScene");
            targetClass.TypeAttributes = System.Reflection.TypeAttributes.Class | System.Reflection.TypeAttributes.Public;

            AddConstantValue(targetClass, typeof(string), "SceneName", sceneName);

            var loadingParameters = SceneAnalyzer.GetLoadingParameters(scene.Scene);
            var haveUnloading = SceneAnalyzer.CheckUnloadingLoadingScene(scene.Scene);

            foreach (var loadingParameter in loadingParameters)
            {
                AddLoadingMethod(targetClass, loadingParameter);

                if (haveUnloading)
                {
                    AddLoadingMethodWithLoadingScreen(targetClass, loadingParameter);
                }
            }

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
            pathConstant.Attributes = MemberAttributes.Public | MemberAttributes.Const;
            pathConstant.InitExpression = new CodePrimitiveExpression(value);
            targetClass.Members.Add(pathConstant);
        }

        private static void AddLoadingMethod(CodeTypeDeclaration targetClass, Type parameterType = null)
        {
            var loadMethod = new CodeMemberMethod();
            loadMethod.Name = "Load";
            loadMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            var loadSceneStatement = "LoadScene(SceneName, loadSceneMode)";

            if (parameterType != null)
            {
                var parameter = new CodeParameterDeclarationExpression(parameterType, "argument");
                loadMethod.Parameters.Add(parameter);
                loadSceneStatement = "LoadScene(argument, SceneName, loadSceneMode)";
            }

            var loadingModeParameter = new CodeParameterDeclarationExpression(nameof(LoadSceneMode), "loadSceneMode = LoadSceneMode.Single");
            loadMethod.Parameters.Add(loadingModeParameter);

            loadMethod.Statements.Add(new CodeSnippetExpression(loadSceneStatement));
            targetClass.Members.Add(loadMethod);
        }

        private static void AddLoadingMethodWithLoadingScreen(CodeTypeDeclaration targetClass, Type parameterType = null)
        {
            var loadMethod = new CodeMemberMethod();
            loadMethod.Name = "LoadSceneWithLoadingScreen";
            loadMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            var loadSceneWithLoadingScreen = "LoadSceneWithLoadingScreen(_sceneName, loadingScreenName)";

            if (parameterType != null)
            {
                var parameter = new CodeParameterDeclarationExpression(parameterType, "argument");
                loadMethod.Parameters.Add(parameter);
                loadSceneWithLoadingScreen = "LoadSceneWithLoadingScreen(argument, SceneName, loadingScreenName)";
            }

            var loadingModeParameter = new CodeParameterDeclarationExpression("System.String", "loadingScreenName");
            loadMethod.Parameters.Add(loadingModeParameter);

            loadMethod.Statements.Add(new CodeSnippetExpression(loadSceneWithLoadingScreen));
            targetClass.Members.Add(loadMethod);
        }
    }
}
#endif
