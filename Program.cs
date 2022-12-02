using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var options = new JsonSerializerOptions
{
  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
  // Don't encode Danish characters 
  Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
  // Pretty print
  WriteIndented = true
};

app.MapGet("/", () => Results.Redirect("/holidays"));
app.MapGet("/holidays", () => Results.Text(DkHolidayCalendar.GetDanishHolidaysString()));
app.MapGet("/holidays/{year}", (int year) => Results.Text(DkHolidayCalendar.GetDanishHolidaysString(year)));

app.Run();
