using Dicgo.Domain.Attributes;
using Dicgo.Domain.Enums;
using Dicgo.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dicgo.Common.Helpers
{
    public class ToolHelper
    {
        public static List<ToolModel> Tools { get; private set; } = new List<ToolModel>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="colorDic"></param>
        public static void Init(AppDomain appDomain,Dictionary<string, string> colorDic)
        {
            Tools.Clear();
            var types = GetAlTypes(appDomain);
            foreach (var item in types)
            {
                var toolAttribute =
               (ToolAttribute)Attribute.GetCustomAttribute(item, typeof(ToolAttribute));

                ToolModel toolModel = new ToolModel();
                toolModel.Icon = toolAttribute.Icon;
                toolModel.SortId = toolAttribute.SortId;
                toolModel.ToolName = toolAttribute.ToolName.GetLangText();
                toolModel.IsVisible = toolAttribute.IsVisible;
                toolModel.ClassificationType = toolAttribute.ClassificationType;
                toolModel.ViewType = toolAttribute.ViewType;
                toolModel.ToolType = toolAttribute.ToolType;
                toolModel.ViewModelType = toolAttribute.ViewModelType;
                if (colorDic != null && colorDic.ContainsKey(toolAttribute.ToolType))
                {
                    toolModel.Color = colorDic[toolAttribute.ToolType];
                }
                Tools.Add(toolModel);

            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private static Type[] GetAlTypes(AppDomain appDomain)
        {
            var allTypes = appDomain.GetAssemblies()
              .SelectMany(a => a.GetTypes().Where(t => t.GetCustomAttributes(false).ToList().Exists(m => m is ToolAttribute)))
              .ToArray();
            return allTypes;
        }
    }
}
