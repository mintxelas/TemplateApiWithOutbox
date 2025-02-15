using System;
using System.Text.Json.Serialization;

namespace Sample.Api.Models;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
}