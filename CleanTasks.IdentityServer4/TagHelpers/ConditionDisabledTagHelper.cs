using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityServer4.Quickstart.UI
{
    [HtmlTargetElement(Attributes = ClassPrefix)]
    public class ConditionDisabledTagHelper : TagHelper
    {
        private const string ClassPrefix = "condition-disabled";

        [HtmlAttributeName(ClassPrefix)]
        public bool Condition { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Condition) output.Attributes.Add("disabled", "true");
        }
    }
}
