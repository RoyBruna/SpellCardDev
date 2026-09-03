using UnityEngine;

namespace SpellCardDev.Core
{
    /// <summary>
    /// Genera sprites circulares en memoria usados como placeholder.
    /// Todos los scripts del juego que usen sprites tienen un [SerializeField]
    /// para reemplazarlos manualmente desde el Inspector.
    /// </summary>
    public static class SpriteGenerator
    {
        /// <summary>
        /// Crea un sprite circular de radio dado con el color indicado.
        /// Se genera como Texture2D en memoria — no se guarda en disco.
        /// </summary>
        public static Sprite CreateCircle(int radius, Color color, Color? glowColor = null)
        {
            int size = radius * 2;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            Color transparent = new Color(0f, 0f, 0f, 0f);
            Vector2 center = new Vector2(radius - 0.5f, radius - 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);

                    if (dist <= radius)
                    {
                        // Glow suave en el borde exterior
                        float t = dist / radius;
                        Color c = glowColor.HasValue
                            ? Color.Lerp(color, glowColor.Value, Mathf.Pow(t, 2f))
                            : color;

                        // Suavizado de borde (antialiasing manual)
                        float alpha = Mathf.Clamp01(1f - Mathf.Max(0f, dist - (radius - 1f)));
                        c.a = alpha;
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, transparent);
                    }
                }
            }

            tex.Apply();

            Rect rect = new Rect(0, 0, size, size);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(tex, rect, pivot, size);
        }

        /// <summary>
        /// Crea un sprite de polígono simple (nave del jugador placeholder).
        /// Triángulo apuntando hacia arriba con color dado.
        /// </summary>
        public static Sprite CreateShipShape(int size, Color bodyColor, Color accentColor)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            Color transparent = new Color(0f, 0f, 0f, 0f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, transparent);
                }
            }

            // Dibujar triángulo simple apuntando hacia arriba
            float cx = size / 2f;
            for (int y = 0; y < size; y++)
            {
                float t = (float)y / size;                    // 0=abajo, 1=arriba
                float halfWidth = cx * (1f - t) * 0.9f;
                int xMin = Mathf.RoundToInt(cx - halfWidth);
                int xMax = Mathf.RoundToInt(cx + halfWidth);

                for (int x = xMin; x <= xMax; x++)
                {
                    if (x < 0 || x >= size) continue;
                    // Acento en los bordes laterales (simulación de cabina)
                    bool isEdge = (x == xMin || x == xMax);
                    bool isCabin = (Mathf.Abs(x - cx) < cx * 0.3f && t > 0.5f);
                    Color c = isEdge ? accentColor : (isCabin ? accentColor * 0.7f + bodyColor * 0.3f : bodyColor);
                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            Rect rect = new Rect(0, 0, size, size);
            return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), size);
        }
    }
}
