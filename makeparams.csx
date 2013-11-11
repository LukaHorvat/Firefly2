using (var writer = new StreamWriter("params.json"))
using (var reader = new StreamReader("body.txt"))
{
	writer.Write("{\"name\": \"Firefly 2\",\"tagline\": \"A new, clean, component-based version of FireflyGL, the C#/OpenGL 2D rendering and game engine.\",\"body\":");
	writer.Write("\"" + reader.ReadToEnd()
	.Replace("\n", "\\n")
	.Replace("\t", "\\t")
	.Replace("\r", "\\r")
	.Replace("\"", "\\\"") + "\"");
	writer.Write(",\"google\": \"\",\"note\": \"Don't delete this file! It's used internally to help with page regeneration.\"}");
}