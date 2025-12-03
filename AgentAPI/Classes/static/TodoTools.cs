using QuickType;
using System.Collections.Generic;

namespace TemplateAPI.Classes;

public static class TodoTools {
    public static Tool GetTodoListTool() => new Tool {
        Type = "function",
        Function = new CalledFunction {
            Name = "todo_list",
            Description = "Add to the todo list for an AI agent to stay focused.",
            Parameters = new Parameters {
                Type = "object",
                Properties = new Dictionary<string, ParameterDescription> {
                    ["item"] = new ParameterDescription {
                        Type = "string",
                        Description = "The todo list item to add to the array."
                    },
                },
                ParametersRequired = ["item"]
            }
        }
    };

    public static Tool GetTopLevelFoldersTool() => new Tool {
        Type = "function",
        Function = new CalledFunction {
            Name = "get_top_level_folders",
            Description = "Retrieve the list of top-level folders in the data repository.",
            Parameters = new Parameters {
                Type = "object",
                Properties = new Dictionary<string, ParameterDescription>(),
                ParametersRequired = []
            }
        }
    };

    public static Tool GetFilesInFolderTool() => new Tool {
        Type = "function",
        Function = new CalledFunction {
            Name = "get_files_in_folder",
            Description = "Retrieve the list of file paths in a specific folder within the data repository.",
            Parameters = new Parameters {
                Type = "object",
                Properties = new Dictionary<string, ParameterDescription> {
                    ["folder"] = new ParameterDescription {
                        Type = "string",
                        Description = "The name of the folder to retrieve files from."
                    },
                },
                ParametersRequired = ["folder"]
            }
        }
    };

    public static Tool GetFileContentsTool() => new Tool {
        Type = "function",
        Function = new CalledFunction {
            Name = "get_file_contents",
            Description = "Retrieve the contents of a specific file within a folder in the data repository.",
            Parameters = new Parameters {
                Type = "object",
                Properties = new Dictionary<string, ParameterDescription> {
                    ["folder"] = new ParameterDescription {
                        Type = "string",
                        Description = "The name of the folder containing the file."
                    },
                    ["relpath"] = new ParameterDescription {
                        Type = "string",
                        Description = "The relative path to the file within the folder."
                    },
                },
                ParametersRequired = ["folder", "relpath"]
            }
        }
    };
}