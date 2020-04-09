﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuanNiao.Blazor.Core;

namespace LuanNiao.Blazor.Component.Antd.Menu
{
    public partial class Item : LNBCBase
    {
        private const string _disableClassName = "ant-menu-item-disabled";
        private const string _inGroupClassName = "ant-menu-item-only-child";
        private const string _selectedClassName = "ant-menu-item-selected";
        public Item()
        {
            _classHelper.SetStaticClass("ant-menu-item");
        }


        [Parameter]
        public bool Disabled { get; set; }

        private string _key = null;
        private bool _selected = false;


        [Parameter]
        public string Key
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_key))
                {
                    return IdentityKey;
                }
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        [Parameter]
        public string Title { get; set; }


        [CascadingParameter]
        public MenuBase RootMenuInstance { get; set; }


        [CascadingParameter]
        public ItemGroup ItemGroup { get; set; }

        [CascadingParameter]
        public SubMenu ParentSubMenu { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            HandleDisable();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            HandleInGroup();
            HandleInInlineMenu();
            RootMenuInstance.ItemSelected += GotOtherItemSelectedEvent;
            if (RootMenuInstance is InlineMenu inlineMenu)
            {
                inlineMenu.CollapsedStatusChanged += (status) =>
                { 
                    this._classHelper.AddCustomClass(_selectedClassName, () => this._selected);
                    this.Flush();
                    Console.WriteLine("set select");
                };
            }
        }

        private void HandleInInlineMenu()
        {
            if (RootMenuInstance is InlineMenu inlineMenu && !inlineMenu.Collapsed)
            {
                var depth = 1;
                GetSubDepth(ref depth, this.ParentSubMenu);
                this._styleHelper.AddCustomStyle("padding-left", $"{depth * 24}px");
            }
        }

        private void GetSubDepth(ref int depth, SubMenu parent)
        {
            if (parent == null)
            {
                return;
            }
            if (parent.ParentSubmenu != null)
            {
                GetSubDepth(ref depth, parent.ParentSubmenu);
            }
            depth += 1;
        }

        private void GotOtherItemSelectedEvent(Item targetItem)
        {
            if (targetItem.Equals(this))
            {
                return;
            }
            this._classHelper.RemoveCustomClass(_selectedClassName);
            this.Flush();
        }

        private void HandleClickEvent()
        {
            if (this.Disabled)
            {
                return;
            }
            this._selected = !this._selected;
            this._classHelper.AddCustomClass(_selectedClassName);
            this.RootMenuInstance.Triggered(this);
        }

        private void HandleInGroup()
        {
            if (ItemGroup != null)
            {
                _classHelper.AddCustomClass(_inGroupClassName);
            }
        }

        private void HandleDisable()
        {
            if (Disabled)
            {
                _classHelper.AddCustomClass(_disableClassName);
            }
            else
            {
                _classHelper.RemoveCustomClass(_disableClassName);
            }
        }
    }
}
