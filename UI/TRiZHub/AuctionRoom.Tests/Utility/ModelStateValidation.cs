using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TRiZHub.Tests.Utility
{

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class ModelStateValidation
   
    {
        public static void ValidateModel(this Controller controller, object viewModel)
        {
            controller.ModelState.Clear();
            var validationContext = new ValidationContext(viewModel, null, null);
            var validationResult = new List<ValidationResult>();
            Validator.TryValidateObject(viewModel, validationContext, validationResult);
            foreach (var result in validationResult)
            {
                foreach (var name in result.MemberNames)
                {
                    controller.ModelState.AddModelError(result.MemberNames.First(), result.ErrorMessage);
                }

            }
        }
    }
}
