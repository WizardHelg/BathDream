using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace BathDream.TagHelpers
{
    public class SimpleFormContainer : TagHelper
    {
        public string Label { get; set; }
        public string ColSmWidth { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "form-group row");
            output.PreContent.SetHtmlContent($"\n<label class=\"col-sm-3 control-label\">{Label}</label>\n<div class=\"col-sm-{ColSmWidth}\">");
            output.PostContent.SetHtmlContent("</div>\n");
        }
    }
}
