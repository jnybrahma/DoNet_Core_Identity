using Microsoft.AspNetCore.Authorization;

namespace WebApp_IdentityExample.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }

    }

    // abstract class
    public class HRManagerProbingRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
           if( !context.User.HasClaim( x => x.Type == "EmploymentDate"))
                return Task.CompletedTask;

           if(DateTime.TryParse(context.User.FindFirst( x => x.Type == "EmploymentDate")?.Value, out DateTime employmentDate))
           {
                var period = DateTime.Now - employmentDate;
                if (period.Days > 1 * requirement.ProbationMonths)
                    context.Succeed(requirement);
           }

            return Task.CompletedTask;
        }
    }
}
