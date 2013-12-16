namespace SobekCM.Resource_Object
{
    /// <summary> Progress delegate used for general progress updates in this library </summary>
    /// <param name="task"> Text of the current task </param>
    /// <param name="current_value"> Current progress value </param>
    /// <param name="maximum_value"> Maximum progress value for the current set of tasks </param>
    public delegate void New_SobekCM_Bib_Package_Progress(string task, int current_value, int maximum_value);

    /// <summary> Progress delegate used for general progress updates in this library </summary>
    /// <param name="task_group"> Text for the current set of tasks </param>
    /// <param name="task"> Text of the current task </param>
    /// <param name="current_value"> Current progress value </param>
    /// <param name="maximum_value"> Maximum progress value for the current set of tasks </param>
    public delegate void New_SobekCM_Bib_Package_Progress_Task_Group(string task_group, string task, int current_value, int maximum_value);
}