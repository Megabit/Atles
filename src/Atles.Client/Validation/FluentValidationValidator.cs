//using FluentValidation;
//using FluentValidation.Internal;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Forms;
//using Microsoft.Extensions.Localization;
//using System;

//namespace Atles.Client.Validation
//{
//    /// <summary>
//    /// Customisation of https://github.com/Blazored/FluentValidation
//    /// </summary>
//    public class FluentValidationValidator : ComponentBase
//    {
//        [Inject] private IServiceProvider ServiceProvider { get; set; }
//        //[Inject] public IStringLocalizer<ValidationResources> StringLocalizer { get; set; }

//        [CascadingParameter] private EditContext CurrentEditContext { get; set; }

//        [Parameter] public IValidator Validator { get; set; }

//        internal Action<ValidationStrategy<object>> Options;

//        public bool Validate(Action<ValidationStrategy<object>> options)
//        {
//            Options = options;

//            try
//            {
//                return CurrentEditContext.Validate();
//            }
//            finally
//            {
//                Options = null;
//            }
//        }

//        protected override void OnInitialized()
//        {
//            if (CurrentEditContext == null)
//            {
//                throw new InvalidOperationException($"{nameof(FluentValidationValidator)} requires a cascading " +
//                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(FluentValidationValidator)} " +
//                    $"inside an {nameof(EditForm)}.");
//            }

//            CurrentEditContext.AddFluentValidation(ServiceProvider, Validator, this);
//        }
//    }
//}