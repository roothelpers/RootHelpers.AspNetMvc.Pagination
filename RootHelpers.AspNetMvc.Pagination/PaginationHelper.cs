using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RootHelpers.AspNetMvc.Pagination
{
    /// <summary>
    /// Add pagination links to help split up your long content into shorter, easier to understand blocks.
    /// </summary>
    public static class PaginationHelper
    {
        /// <summary>
        /// Obtient le nombre de page
        /// </summary>
        /// <param name="CountResultsTotal">The count results total.</param>
        /// <param name="ShowCountResults">nombre de résultat par page</param>
        /// <returns></returns>
        public static int CountPages(int CountResultsTotal, int ShowCountResults)
        {
            return (int)Math.Ceiling((decimal)CountResultsTotal / (decimal)ShowCountResults);
        }

        /// <summary>
        /// Obtien le offset (position de départ des résultats) en fonction de la page en cours
        /// </summary>
        /// <param name="CurrentPage">The current page.</param>
        /// <param name="ShowCountResults">The show count results.</param>
        /// <returns></returns>
        public static int OffsetPageResults(int CurrentPage, int ShowCountResults)
        {
            if (CurrentPage == 0 || CurrentPage == 1) return 0;
            if (ShowCountResults == 0) return 0;
            return (CurrentPage * ShowCountResults) - ShowCountResults;
        }

        /// <summary>
        /// Shows the result count.
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString ShowResultCount()
        {
            return new MvcHtmlString("");
        }

        /// <summary>
        /// Shows the pagination.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="form">The form.</param>
        /// <param name="CountResultsTotal">The count results total.</param>
        /// <param name="SubmitFormId">The submit form identifier.</param>
        /// <returns></returns>
        public static MvcHtmlString ShowPagination<TModel>(this HtmlHelper<TModel> html, int currentPage, int CountResultsTotal, int LimitResults, string SubmitFormId, string LanguageDirection)
        {
            StringBuilder retour = new StringBuilder(); // Unlike a string, a StringBuilder can be changed. With it, an algorithm that modifies characters in a loop runs fast.
            int nbpageRange = 4;
            int totalpage = CountPages(CountResultsTotal, LimitResults);
            //if (currentPage == 0) currentPage = 1;
            //string jsHrefFormatString = " onClick=\"location.href=URL_add_parameter(location.href,'CurrentPage',{0})\" ";
            string jsHrefFormatString = "  ";

            retour.Append(" <ul id=" + SubmitFormId + " class='pagination'style='display: inline-flex; color:#dbb34a ;" + (LanguageDirection == "rtl" ? "direction: rtl; " : "") + "'>");
            if (currentPage > 1)
                retour.AppendFormat("<li><a " + jsHrefFormatString + " id='beginpage' title='Previous page'>&laquo;</a></li>");

            int countallwaysshow = 3; // nombres de pages des bors exterieurs a toujours afficher
            bool showspace = false;
            for (int iipage = 1; iipage <= totalpage; iipage++)
            {
                if (iipage > countallwaysshow && iipage <= (totalpage - countallwaysshow)) // on affiche toujours les pages des bords extérieurs
                    if (!(currentPage < countallwaysshow && iipage < countallwaysshow * 2 + 1)) // on affiche quand même un minimum de résultats
                        if (iipage < (currentPage - nbpageRange) || iipage > (currentPage + nbpageRange)) // on affiche toujours les pages proche de la current page
                        {
                            showspace = true;
                            continue; // cache les pages en trops pour optimiser l'affichage
                        }

                if (currentPage == iipage) retour.AppendFormat("<li class='active'><a href='#'>{0}</a></li>", iipage);
                else
                {
                    if (showspace)
                    {
                        showspace = false;
                        retour.AppendFormat("<li><a " + jsHrefFormatString + " style='margin-left:10px;' >{0}</a></li>", iipage);
                    }
                    else retour.AppendFormat("<li><a " + jsHrefFormatString + " >{0}</a></li>", iipage);
                }
            }

            if (currentPage < totalpage)
                retour.AppendFormat("<li><a  id='endpage' " + jsHrefFormatString + "title='Next page'>&raquo;</a></li>");

            retour.Append("</ul>");
            retour.AppendLine();

            return new MvcHtmlString(retour.ToString()); // This returns the buffer. We will almost always want ToString—it will return the contents as a string.
        }

        public static MvcHtmlString PaginationJS(String UrLAction, int totalcount, int pagesize, String IDdiv, String div, String divloader)
        {
            StringBuilder retour = new StringBuilder(); // Unlike a string, a StringBuilder can be changed. With it, an algorithm that modifies characters in a loop runs fast.
            int totalpage = CountPages(totalcount, pagesize);
            retour.AppendLine("<script>");
            retour.AppendLine("$(document).ready(function() {");
            retour.AppendLine("$('#" + IDdiv + ".pagination li a').click(function(e) {");
            retour.AppendLine(" e.preventDefault();");
            retour.AppendLine("var pagenumber = 1; if ($(this).attr('id') == 'endpage') { pagenumber =" + totalpage + "; } else if ($(this).attr('id') == 'beginpage') { pagenumber =1; } else { pagenumber = parseInt($(this).html()); }");
            retour.AppendLine("$.ajax({");
            retour.AppendLine("url: '" + UrLAction + "',");
            retour.AppendLine(" data: { page: pagenumber },");
            retour.AppendLine("beforeSend: function() {");
            retour.AppendLine(" $('#" + div + "').html('');");
            retour.AppendLine("$('#" + divloader + "').show();");
            retour.AppendLine("},");
            retour.AppendLine("complete: function() {");
            retour.AppendLine("$('#" + divloader + "').hide();");
            retour.AppendLine("},");
            retour.AppendLine("success: function(data) {");
            retour.AppendLine(" $('#" + div + "').html(data);");
            retour.AppendLine("},");
            retour.AppendLine("error: function() {");
            retour.AppendLine("}");
            retour.AppendLine("});");
            retour.AppendLine("})");
            retour.AppendLine("});");
            retour.AppendLine("</script>");

            return new MvcHtmlString(retour.ToString()); // This returns the buffer. We will almost always want ToString—it will return the contents as a string.
        }



    }
}