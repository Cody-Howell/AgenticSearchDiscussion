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
}