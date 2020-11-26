using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BathDream.TagHelpers
{
    public class RadioFormElement : TagHelper
    {
        public string ElementName { get; set; }
        public string ElementValue { get; set; }
        public bool ElementChecked { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "lable";
            output.Attributes.SetAttribute("class", "radio-inline");
            output.PreContent.SetHtmlContent($"\n<input type=\"radio\" name=\"{ElementName}\" value=\"{ElementValue}\" {(ElementChecked ? "checked " : "")}/>\n");
        }
    }
}
