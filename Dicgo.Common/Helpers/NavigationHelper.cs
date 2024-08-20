using Dicgo.Domain.CacheModels;
using Dicgo.Domain.Interfaces;
using Dicgo.Domain.Models;
using Dicgo.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Dicgo.Common.Helpers
{
    public class NavigationHelper
    {

        /// <summary>
        /// 使用缓存
        /// </summary>
        private static bool isUseCache { get; set; }
        /// <summary>
        /// 设置使用缓存
        /// </summary>
        /// <param name="hasUse"></param>
        public static void SetUseCache(bool hasUse)
        {
            isUseCache = hasUse;
        }
        public static void SetTools(List<ToolModel> toolModels)
        {
            NavigationCacheDic.Clear();
            Tools = toolModels;
        }
        private static List<ToolModel> Tools { get; set; } = new List<ToolModel>();
        /// <summary>
        /// 页面缓存
        /// </summary>
        public static Dictionary<string, NavigationModel> NavigationCacheDic = new Dictionary<string, NavigationModel>();

        private static FrameworkElement? WorkView { get; set; }
        /// <summary>
        /// 设置工作界面
        /// </summary>
        /// <param name="framework"></param>
        public static void SetWorkView(FrameworkElement framework)
        {
            WorkView = framework;
        }
        /// <summary>
        /// 导航到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewKey"></param>
        /// <param name="param"></param>
        /// <param name="cacheKey"></param>
        public static void GoTo<T>(string viewKey, T param, string cacheKey = "Dicgo")
        {
            GoTo(WorkView, viewKey, param, cacheKey);
        }


        public static void GoTo<T>(FrameworkElement? workView, string viewKey, T param, string cacheKey = "Dicgo")
        {
            if (workView != null && workView.DataContext != null)
            {
                var naviget = workView.DataContext as INavigateOut;
                if (naviget != null)
                    naviget.NavigateOut();
            }
            workView = GetView(viewKey, param, cacheKey).View ?? null;
        }
        /// <summary>
        /// 获取key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewKey"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static NavigationModel? GetView<T>(string viewKey, T param, string cacheKey = "Dicgo")
        {

            NavigationModel navigationModel = new NavigationModel();
            if (isUseCache)
            {
                if (!string.IsNullOrWhiteSpace(cacheKey) && NavigationCacheDic.ContainsKey(cacheKey))
                {
                    navigationModel = NavigationCacheDic[cacheKey];
                    if (navigationModel != null)
                    {
                        Navigate(param, navigationModel);
                        return navigationModel;
                    }
                }
            }
            var model = Tools.Find(p => p.ToolType == viewKey) ?? null;
            if (model == null || model.ViewType == null || model.ViewModelType == null) return navigationModel;
            var view = Activator.CreateInstance(model.ViewType) as FrameworkElement;
            var vm = Activator.CreateInstance(model.ViewModelType) as BaseViewModel;
            if (view != null)
                view.DataContext = vm;
            if (navigationModel == null)
                navigationModel = new NavigationModel();
            if (view != null && vm != null)
            {
                navigationModel.View = view;
                navigationModel.ViewModel = vm;
                if (isUseCache)
                {
                    if (!NavigationCacheDic.ContainsKey(cacheKey))
                        NavigationCacheDic.Add(cacheKey, navigationModel);
                    else NavigationCacheDic[cacheKey] = navigationModel;
                }
            }

            return navigationModel;
        }
        /// <summary>
        /// 导航到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="navigationModel"></param>
        private static void Navigate<T>(T param, NavigationModel navigationModel)
        {
            var navigate = navigationModel.ViewModel as INavigateIn;

            if (navigate != null)
            {
                navigate.NavigateIn();
            }
            else
            {
                var navigateParam = navigationModel.ViewModel as INavigateIn<T>;
                if (navigateParam != null)
                    navigateParam.NavigateIn(param);
            }
        }
    }
}
