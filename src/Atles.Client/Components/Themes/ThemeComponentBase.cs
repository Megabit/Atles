﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Atlas.Client.Services;
using Atlas.Models.Public;
using Atles.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Themes
{
    public abstract class ThemeComponentBase : ComponentBase
    {
        [CascadingParameter] protected CurrentUserModel User { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        public bool SavingData { get; set; }
        protected string CssClassDisabled => SavingData ? "disabled" : string.Empty;

        protected async Task<HttpResponseMessage> SaveDataAsync(Func<Task<HttpResponseMessage>> actionAsync)
        {
            SavingData = true;
            var response = await actionAsync();
            SavingData = false;
            return response;
        }
    }
}