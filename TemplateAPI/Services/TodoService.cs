namespace TemplateAPI.Services;

using System.Collections.Generic;
using System.Linq;
using TemplateAPI.Classes;

public class TodoService {
    private readonly Dictionary<int, List<TodoItem>> todos = [];

    public TodoItem AddTodoItem(int id, string item) {
        if (!todos.TryGetValue(id, out List<TodoItem>? value)) {
            value = new List<TodoItem>();
            todos.Add(id, value);
        }

        var todo = new TodoItem {
            Id = Guid.NewGuid(),
            Text = item,
        };
        value.Add(todo);
        return todo;
    }

    public List<TodoItem> GetTodoItems(int id) {
        if (todos.TryGetValue(id, out List<TodoItem>? value)) {
            return value;
        }
        return new List<TodoItem>();
    }

    public TodoItem? GetOldestItem(int id) {
        if (todos.TryGetValue(id, out List<TodoItem>? value) && value.Count != 0) {
            return value.First();
        }
        return null;
    }

    public bool DeleteOldestItem(int id) {
        if (!todos.TryGetValue(id, out List<TodoItem>? list)) return false;
        return DeleteTodoItem(id, list.First().Id);
    }

    public bool DeleteTodoItem(int id, Guid itemId) {
        if (!todos.TryGetValue(id, out List<TodoItem>? list)) return false;
        var item = list.FirstOrDefault(t => t.Id == itemId);
        if (item == null) return false;

        list.Remove(item);
        return true;
    }
}