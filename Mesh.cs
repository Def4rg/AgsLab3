using System.Globalization;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
namespace GraphicApllication;

public class Mesh
{

    private class Vertex
    {
        private Vector3? _coord { get; set; }
        private Vector2? _texCoord { get; set; }
        private Vector3? _normal { get; set; }

        public Vertex(Vector3 coord, Vector2 texCoord, Vector3 normal)
        {
            _coord = coord;
            _normal = normal;
            _texCoord = texCoord;
        }

        public Vertex()
        {
            _coord = new Vector3();
            _normal = new Vector3();
            _texCoord = new Vector2();
        }
    }

    private int _vao;
    private int _vertexBuffer;
    private int _indexBuffer;
    private int _vertexCount;

    public Mesh()
    {
        
    }

    public bool Load(string filepath)
    {
        if (!File.Exists(filepath)) return false;
        List<Vector3> vertexs = new();
        List<Vector2> texturs = new();
        List<Vector3> normals = new();
        List<Vector3> fpoints = new();
        string[] filelines = File.ReadAllLines(filepath).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        foreach (var line in filelines)
        {
            string[] parames = line.Split(' ');
            switch (parames[0])
            {
                case "v":
                    vertexs.Add(new Vector3(float.Parse(parames[1], CultureInfo.InvariantCulture), float.Parse(parames[2], CultureInfo.InvariantCulture), float.Parse(parames[3], CultureInfo.InvariantCulture)));
                    break;
                case "vt":
                    texturs.Add(new Vector2(float.Parse(parames[1], CultureInfo.InvariantCulture), float.Parse(parames[2], CultureInfo.InvariantCulture)));
                    break;
                case "vn":
                    normals.Add(new Vector3(float.Parse(parames[1], CultureInfo.InvariantCulture), float.Parse(parames[2], CultureInfo.InvariantCulture), float.Parse(parames[3], CultureInfo.InvariantCulture)));
                    break;
                case "f":
                    for (int i = 1; i < 4; i++)
                    {
                        float[] points = parames[i].Split('/').Select(value => float.Parse(value)).ToArray();
                        fpoints.Add(new Vector3(points));
                    }
                    break;
            }
        }

        int index = 0;
        List<int> indices = new();
        Dictionary<Vector3, int> vertexToIndexTable = new ();
        List<float> vertices = new();
        for (int i = 0; i < fpoints.Count; i++)
        {
            if(vertexToIndexTable.ContainsKey(fpoints[i]))indices.Add(vertexToIndexTable[fpoints[i]]);
            else
            {
                vertexToIndexTable.Add(fpoints[i], index);
                indices.Add(vertexToIndexTable[fpoints[i]]);
                //vertices.Add(new Vertex(vertexs[(int)fpoints[i].X - 1], texturs[(int)fpoints[i].Y - 1], normals[(int)fpoints[i].Z - 1]));
                
                vertices.Add(vertexs[(int)fpoints[i].X - 1].X);
                vertices.Add(vertexs[(int)fpoints[i].X - 1].Y);
                vertices.Add(vertexs[(int)fpoints[i].X - 1].Z);
                vertices.Add(texturs[(int)fpoints[i].Y - 1].X);
                vertices.Add(texturs[(int)fpoints[i].Y - 1].Y);
                vertices.Add(normals[(int)fpoints[i].Z - 1].X);
                vertices.Add(normals[(int)fpoints[i].Z - 1].Y);
                vertices.Add(normals[(int)fpoints[i].Z - 1].Z);

                index++;
            }
        }
        _vertexCount = vertices.Count();
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _vertexCount, vertices.ToArray(), BufferUsageHint.StaticDraw);
        
        _indexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Count, indices.ToArray(), BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, 12);
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 5 * sizeof(float));
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        GL.BindVertexArray(0);

        return true;
    }

    public void DrawOne()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }
    public void DrawMany(int count)
    {
        GL.BindVertexArray(_vao);
        GL.DrawElementsInstanced(PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, (IntPtr)0, count);
        GL.BindVertexArray(0);
    }
}