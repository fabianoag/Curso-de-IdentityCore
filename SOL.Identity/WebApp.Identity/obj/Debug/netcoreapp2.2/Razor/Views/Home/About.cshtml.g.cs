#pragma checksum "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\Home\About.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2cd447181f9d0fda973b32d5e82acd58e47499b0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_About), @"mvc.1.0.view", @"/Views/Home/About.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/About.cshtml", typeof(AspNetCore.Views_Home_About))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\_ViewImports.cshtml"
using WebApp.Identity;

#line default
#line hidden
#line 2 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\_ViewImports.cshtml"
using WebApp.Identity.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2cd447181f9d0fda973b32d5e82acd58e47499b0", @"/Views/Home/About.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"74a8383de03762724feed4c96ff772cb29145fc9", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_About : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\Home\About.cshtml"
  
    ViewData["Title"] = "About";

#line default
#line hidden
            BeginContext(43, 24, true);
            WriteLiteral("\r\n<h2>About</h2>\r\n<ul>\r\n");
            EndContext();
#line 8 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\Home\About.cshtml"
     foreach (var claim in User.Claims)
    {

#line default
#line hidden
            BeginContext(115, 15, true);
            WriteLiteral("        <li><b>");
            EndContext();
            BeginContext(131, 10, false);
#line 10 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\Home\About.cshtml"
          Write(claim.Type);

#line default
#line hidden
            EndContext();
            BeginContext(141, 6, true);
            WriteLiteral("</b>: ");
            EndContext();
            BeginContext(148, 11, false);
#line 10 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\Home\About.cshtml"
                           Write(claim.Value);

#line default
#line hidden
            EndContext();
            BeginContext(159, 7, true);
            WriteLiteral("</li>\r\n");
            EndContext();
#line 11 "I:\Aula atual Udemy\Asp.NET Identity\Seção 6\SOL.Identity\WebApp.Identity\Views\Home\About.cshtml"
    }

#line default
#line hidden
            BeginContext(173, 9, true);
            WriteLiteral("</ul>\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591