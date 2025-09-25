using CommunityToolkit.Mvvm.Input;
using Maui.Samples.Models;

namespace Maui.Samples.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}