using RulesEngine.Models;
using System.Collections.Generic;

namespace RulesEngine.Interfaces
{
    public interface IBaseRulesEngine
    {
        /// <summary>
        /// Adds new workflows to RulesEngine
        /// </summary>
        /// <param name="workflow"></param>
        void AddWorkflow(params Workflow[] Workflows);

        /// <summary>
        /// Removes all registered workflows from RulesEngine
        /// </summary>
        void ClearWorkflows();

        /// <summary>
        /// Removes the workflow from RulesEngine
        /// </summary>
        /// <param name="workflowNames"></param>
        void RemoveWorkflow(params string[] workflowNames);

        /// <summary>
        /// Checks is workflow exist.
        /// </summary>
        /// <param name="workflowName">The workflow name.</param>
        /// <returns> <c>true</c> if contains the specified workflow name; otherwise, <c>false</c>.</returns>
        bool ContainsWorkflow(string workflowName);

        /// <summary>
        /// Returns the list of all registered workflow names
        /// </summary>
        /// <returns></returns>
        List<string> GetAllRegisteredWorkflowNames();
        void AddOrUpdateWorkflow(params Workflow[] Workflows);
    }
}
