using System;
using System.Globalization;
using System.Web.Mvc;

namespace EBLIG.WebUI.DataBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                if (valueResult?.AttemptedValue != null)
                {
                    actualValue = Math.Round(Convert.ToDecimal(valueResult?.AttemptedValue?.Replace(".", ""),
                        CultureInfo.CurrentCulture), 2, MidpointRounding.ToEven);
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}