using System.Web;
using System.Web.Mvc;

namespace cis237Assignment6
{
    // Filters are used to perform an action either before an action method is called, or after an action method runs.

    /* Types of filters
     * 
     * Authorization Filter
     * Implements IAuthorizationFilter
     * [Runs first]
     * Makes security decisions to execute an action method, performing authentication and/or validation of a request and its properties.
     * 
     * Action Filter
     * Implements IActionFilter
     * Wraps an action method execution. Has two methods: OnActionExecuting and OnResultExecuted (the latter which can provide extra data to the action method,
     * inspect the return value, or cancel the exectuion of the action method).
     * 
     * Result Filter
     * Implements IResultFilter
     * Wraps execution of an ActionResult object. 
    */
    // Types of filters
    // - Authorization filters: (implements IAuthorizationFilter) These run first. Makes security decisions about whether to execute an action method, performing authetnetication/validating properties of the request.
    // - Action filters: (implements IActionFilter) Wrap an action method execution. 
    //                   Declares two methods
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
