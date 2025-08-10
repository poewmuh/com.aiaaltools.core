using UnityEngine;
using UnityEngine.UI;

namespace AiaalTools.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/RoundedRect")]
    public class RoundedRect : MaskableGraphic
    {
        [SerializeField] private Texture _texture = default;
        [SerializeField] private Sprite _sprite = default;
        [SerializeField] private bool _isPreserveAspect = false;
        [SerializeField] private bool _readUV = false;
        [SerializeField] private RectTransform _customRectTransform;
        [SerializeField] private float _radius = 5;
        [SerializeField] private bool _topLeft, _topRight, _bottomLeft, _bottomRight;
        [SerializeField, Range(1, 20)] private int _segments = 9;
        [SerializeField] private bool _spriteMode = false;

        public override Texture mainTexture
        {
            get { return Texture ?? s_WhiteTexture; }
        }

        public Texture Texture
        {
            get
            {
                if (_spriteMode)
                {
                    if (_sprite != null)
                        return _sprite.texture;
                }

                return _texture;
            }

            set
            {
                if (_texture != value)
                {
                    _texture = value;
                    SetMaterialDirty();
                }
            }
        }

        public Sprite Sprite
        {
            get
            {
                if (_spriteMode)
                {
                    return _sprite;
                }

                return null;
            }

            set
            {
                if (_sprite != value)
                {
                    _sprite = value;
                    _dirty = true;
                    SetVerticesDirty();
                    SetMaterialDirty();
                }
            }
        }

        private int[] _roots = { 10, 9, 5, 6 };
        private int[] _starts = { 11, 13, 4, 2 };
        private int[] _ends = { 14, 8, 1, 7 };
        private int[] _squares = { 10, 8, 0, 2 };
        private Vector2 _add = Vector2.zero;
        private Vector2 _mul = Vector2.one;

        private Vector3[] _vertices = new Vector3[16];

        private bool[] _buffer = new bool[4];
        private bool[] _oldbuffer = new bool[4];
        private int _oldSegments = -1;
        private Vector2 _oldSize;
        private Vector3 _oldPivot;
        private Vector2 _size;
        private Vector3 _pivot;

        private bool _dirty = false;

        protected override void Awake()
        {
            _dirty = true;
            if (!_customRectTransform)
                _customRectTransform = rectTransform;
        }

        protected override void OnEnable()
        {
            _dirty = true;
            base.OnEnable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _dirty = true;
            base.OnValidate();
        }
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {

            Vector2 textureSize = Vector2.zero;
            if (_spriteMode && _sprite != null)
            {
                if (_readUV && _sprite.packed)
                {
                    Debug.Assert(_sprite.uv.Length == 4,
                        "RoundedRect �� ����� �������� � ���� ��������: �������� ������� Sprite Mode - Mesh Type ������ ���� Full Rect");

                    _add = _sprite.uv[2];
                    _mul = _sprite.uv[1] - _sprite.uv[2];
                }
                else
                {
                    textureSize = new Vector2(_sprite.texture.width, _sprite.texture.height);
                    var spriteOffset = _sprite.textureRect.min;
                    var spriteSize = _sprite.textureRect.size;
                    _add = new Vector2(spriteOffset.x / textureSize.x, spriteOffset.y / textureSize.y);
                    _mul = new Vector2(spriteSize.x / textureSize.x, spriteSize.y / textureSize.y);
                }
            }

            _buffer[0] = _topRight;
            _buffer[1] = _topLeft;
            _buffer[2] = _bottomLeft;
            _buffer[3] = _bottomRight;
            if (_spriteMode && _isPreserveAspect)
            {
                if (_readUV && _sprite.packed)
                {
                    _size = AdjustSize(new Vector2(_mul.x * _sprite.texture.width, _mul.y * _sprite.texture.height),
                        _customRectTransform.rect.size);
                }
                else
                {
                    _size = AdjustSize(textureSize, _customRectTransform.rect.size);
                }
            }
            else
                _size = _customRectTransform.rect.size;

            _pivot = new Vector3(_customRectTransform.pivot.x * _size.x, _customRectTransform.pivot.y * _size.y);
            if (_oldSize != _size)
            {
                _oldSize = _size;
                _dirty = true;
            }

            if (_oldPivot != _pivot)
            {
                _oldPivot = _pivot;
                _dirty = true;
            }

            if (_oldSegments != _segments)
            {
                _oldSegments = _segments;
                _dirty = true;
            }

            for (int i = 0; i < 4; i++)
            {
                if (_buffer[i] != _oldbuffer[i])
                {
                    _oldbuffer[i] = _buffer[i];
                    _dirty = true;
                }
            }

            if (_dirty || vh.currentVertCount != GetTargetVertexCount())
            {
                GenerateMesh(vh);
            }
        }

        private int GetTargetVertexCount()
        {
            int result = 16;
            for (int i = 0; i < 4; i++)
            {
                if (_buffer[i])
                {
                    result += (_segments - 1);
                }
                else
                {
                    result += 2;
                }
            }

            return result;
        }

        private void GenerateMesh(VertexHelper vh)
        {
            _dirty = false;

            vh.Clear();
            DrawBase(vh);

            float angle = 90f / _segments * Mathf.Deg2Rad;

            for (int rootIndex = 0; rootIndex < 4; rootIndex++)
            {
                if (_buffer[rootIndex])
                    DrawSector(vh, rootIndex, angle, _vertices[_roots[rootIndex]]);
                else
                    DrawSquare(vh, _squares[rootIndex]);
            }
        }

        private void DrawSector(VertexHelper vh, int rootIndex, float angle, Vector3 rootPoint)
        {
            int vertexCount = vh.currentVertCount;
            float rad;
            for (int i = 1; i < _segments; i++)
            {
                rad = Mathf.Deg2Rad * 90 * rootIndex + i * angle;
                Vector3 pos = rootPoint + new Vector3(_radius * Mathf.Cos(rad), _radius * Mathf.Sin(rad));
                AddVertex(vh, pos);
            }

            if (_segments >= 2)
            {
                int firstVertex = vertexCount;
                AddTriangle(vh, _roots[rootIndex], _starts[rootIndex], firstVertex);
                for (int i = 0; i < _segments - 2; i++)
                {
                    int a = vertexCount + i;
                    int b = vertexCount + i + 1;
                    AddTriangle(vh, _roots[rootIndex], a, b);
                }

                int lastVertex = vertexCount + _segments - 2;
                AddTriangle(vh, _roots[rootIndex], lastVertex, _ends[rootIndex]);
            }
            else
            {
                AddTriangle(vh, _roots[rootIndex], _starts[rootIndex], _ends[rootIndex]);
            }
        }


        private void DrawBase(VertexHelper vh)
        {
            float[] x = { 0, _radius, _size.x - _radius, _size.x };
            float[] y = { 0, _radius, _size.y - _radius, _size.y };
            for (int i = 0; i < 16; i++)
                _vertices[i] = new Vector3(x[i % 4], y[i / 4]);
            for (int i = 0; i < _vertices.Length; i++)
                AddVertex(vh, _vertices[i]);
            int[] pattern = { 1, 4, 5, 6, 9 };
            for (int i = 0; i < pattern.Length; i++)
                DrawSquare(vh, pattern[i]);
        }

        private void AddVertex(VertexHelper vh, Vector3 vertex)
        {
            vh.AddVert(new UIVertex()
            {
                color = color,
                position = vertex - _pivot,
                uv0 = CorrectedCoord(CalculateUV(_size, vertex))
            });
        }

        private void DrawSquare(VertexHelper vh, int squareIndex)
        {
            if (squareIndex < 15)
            {
                AddTriangle(vh, squareIndex, squareIndex + 1, squareIndex + 5);
                AddTriangle(vh, squareIndex, squareIndex + 5, squareIndex + 4);
            }
        }

        private void AddTriangle(VertexHelper vh, int v1, int v2, int v3)
        {
            vh.AddTriangle(v1, v2, v3);
        }

        protected virtual Vector2 CalculateUV(Vector2 size, Vector3 vertex)
        {
            return new Vector2(vertex.x / size.x, vertex.y / size.y);
        }

        private Vector2 AdjustSize(Vector2 textureSize, Vector2 currentRectSize)
        {
            float scale = textureSize.x > textureSize.y ? currentRectSize.x / textureSize.x : currentRectSize.y / textureSize.y;
            var x = textureSize.x > textureSize.y ? currentRectSize.x : textureSize.x * scale;
            var y = textureSize.x > textureSize.y ? textureSize.y * scale : currentRectSize.y;

            return new Vector2(x, y);
        }

        private Vector2 CorrectedCoord(Vector2 initial)
        {
            if (_spriteMode && (!_isPreserveAspect || _readUV))
            {
                return new Vector2(_add.x + _mul.x * initial.x, _add.y + _mul.y * initial.y);
            }

            return initial;
        }
    }
}
