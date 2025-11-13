namespace TemplateAPI.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using TemplateAPI.Classes;

public class TodoService {
    private readonly Dictionary<int, List<TodoItem>> todos = [];

    public TodoItem AddTodoItem(int id, string item) {
        if (!todos.ContainsKey(id)) {
            todos.Add(id, new List<TodoItem>());
        }

        var todo = new TodoItem {
            Id = Guid.NewGuid(),
            Text = item,
        };

        todos[id].Add(todo);
        return todo;
    }

    public List<TodoItem> GetTodoItems(int id) {
        if (todos.ContainsKey(id)) {
            return todos[id];
        }
        return new List<TodoItem>();
    }

    public TodoItem? GetOldestItem(int id) {
        if (todos.ContainsKey(id) && todos[id].Any()) {
            return todos[id].First();
        }
        return null;
    }

    public bool DeleteTodoItem(int id, Guid itemId) {
        if (!todos.ContainsKey(id)) return false;

        var list = todos[id];
        var item = list.FirstOrDefault(t => t.Id == itemId);
        if (item == null) return false;

        list.Remove(item);
        return true;
    }
}