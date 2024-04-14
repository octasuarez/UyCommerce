using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UYCommerce.Helpers
{
	public class ActiveTagHelper : TagHelper
	{

        public string Url { get; set; }
        public string Action { get; set; }
        public string Classes { get; set; } = "";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var viewAction = ViewContext.RouteData.Values["action"]?.ToString();

            if(String.Equals(Action,viewAction, StringComparison.OrdinalIgnoreCase)) {
                Classes += " active text-white";
            }

            output.TagName = "a";

            output.Attributes.SetAttribute("href",Url);

            output.Attributes.SetAttribute("class", Classes);
        }
    }
}

