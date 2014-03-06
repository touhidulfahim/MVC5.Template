﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Template.Components.Extensions.Html;
using Template.Objects;
using Template.Tests.Helpers;

namespace Template.Tests.Tests.Components.Extensions.Html
{
    [Ignore]
    [TestFixture]
    public class SidebarExtensionsTests
    {
        private HtmlHelper html;

        [SetUp]
        public void SetUp()
        {
            html = new HtmlHelperMock().Html;
        }

        #region Extension method: SidebarSearch(this HtmlHelper html)

        [Test]
        public void SidebarSearch_FormsSidebarSearch()
        {
            var input = new TagBuilder("input");
            input.MergeAttribute("type", "text");
            input.MergeAttribute("id", "SearchInput");
            input.MergeAttribute("placeholder", Template.Resources.Shared.Resources.Search + "...");

            Assert.AreEqual(input.ToString(TagRenderMode.SelfClosing), html.SidebarSearch().ToString());
        }

        #endregion

        #region Extension method: SidebarMenu(this HtmlHelper html, IEnumerable<Menu> menus)

        [Test]
        public void SidebarMenu_FormsSidebarMenu()
        {
            var menus = new List<Menu>()
            {
                new Menu()
                {
                    IconClass = "menu-icon fa fa-home",
                    Controller = "Home",
                    Action = "Index"
                },
                new Menu() {
                    IconClass = "menu-icon fa fa-users",
                    Area = "Administration",
                    IsOpen = true,
                    Submenus = new List<Menu>()
                    {
                        new Menu()
                        {
                            IconClass = "menu-icon fa fa-user",
                            Area = "Administration",
                            Controller = "Users",
                            Action = "Index",
                            IsActive = true,
                            IsOpen = true
                        },
                        new Menu()
                        {
                            IconClass = "menu-icon fa fa-male",
                            Area = "Administration",
                            Controller = "Roles",
                            Action = "Index"
                        }
                    }
                }
            };

            var menuBuilder = new StringBuilder();
            foreach (var menu in menus)
                menuBuilder.Append(Menu(html, menu));


            Assert.AreEqual(menuBuilder.ToString(), html.SidebarMenu(menus).ToString());
        }

        #endregion

        #region Test helpers

        private String Menu(HtmlHelper html, Menu menu)
        {
            TagBuilder menuItem = new TagBuilder("li");
            StringBuilder menuInnerHtml = new StringBuilder();
            menuInnerHtml.Append(FormAction(html, menu));

            if (menu.Submenus.Count() > 0)
            {
                menuItem.AddCssClass("submenu");
                TagBuilder submenus = new TagBuilder("ul");
                StringBuilder innerSubmenuHtml = new StringBuilder();
                foreach (var submenu in menu.Submenus)
                    innerSubmenuHtml.Append(Menu(html, submenu));

                submenus.InnerHtml = innerSubmenuHtml.ToString();
                menuInnerHtml.Append(submenus);
            }

            if (menu.IsActive) menuItem.AddCssClass("active active-hovering");
            if (menu.IsOpen) menuItem.AddCssClass("open");
            menuItem.InnerHtml = menuInnerHtml.ToString();
            return menuItem.ToString();
        }
        private String FormAction(HtmlHelper html, Menu menu)
        {
            TagBuilder menuIcon = new TagBuilder("i");
            TagBuilder span = new TagBuilder("span");
            menuIcon.AddCssClass(menu.IconClass);
            span.InnerHtml = menu.Title;

            if (menu.Action == null)
            {
                TagBuilder openIcon = new TagBuilder("i");
                TagBuilder action = new TagBuilder("a");

                action.InnerHtml = String.Format("{0}{1}{2}", menuIcon, span, openIcon);
                openIcon.AddCssClass("arrow fa fa-chevron-right");

                return action.ToString();
            }

            return String.Format(
                html.ActionLink(
                    "{0}{1}",
                    menu.Action,
                    new
                    {
                        area = menu.Area,
                        controller = menu.Controller
                    }).ToString(),
                menuIcon, span);
        }

	    #endregion
    }
}