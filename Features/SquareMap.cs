using BattleTech;
using UnityEngine;

namespace ISM3025.Features
{
    public static class SquareMap
    {
        public static void OnMapRefresh(StarmapRenderer renderer)
        {
            // set the fov to our settings
            renderer.fovMin = Main.Settings.MinFov;
            renderer.fovMax = Main.Settings.MaxFov;

            ScaleMapBorders(renderer);
        }

        public static void ScaleMapBorders(StarmapRenderer renderer)
        {
            var starmapBorders = renderer.gameObject.GetComponentInChildren<StarmapBorders>();
            if (starmapBorders == null)
                return;

            // set scale of region borders for our new size
            var borderTransform = starmapBorders.gameObject.transform;
            borderTransform.localScale = new Vector3(4f * Main.Settings.MapHeight, 4f * Main.Settings.MapWidth);

            // generate black texture to use for plusTex, so that we don't have a
            // stretched texture, it looked weird anyways
            const int textureSize = 64;
            var blackTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
            for (var x = 0; x < textureSize; x++)
            {
                for (var y = 0; y < textureSize; y++)
                {
                    blackTexture.SetPixel(x, y, Color.black);
                }
            }
            blackTexture.Apply();
            starmapBorders.plusTex = blackTexture;

            // change the map border edges
            var edgeLine = GameObject.Find("Edges")?.GetComponent<LineRenderer>();
            if (edgeLine != null)
            {
                var height = Main.Settings.MapHeight + Main.Settings.BorderMargin;
                var width = Main.Settings.MapWidth + Main.Settings.BorderMargin;

                edgeLine.positionCount = 4;
                edgeLine.SetPositions(new []
                {
                    new Vector3(-height, -width),
                    new Vector3(-height, width),
                    new Vector3(height, width),
                    new Vector3(height, -width),
                });
            }
        }

        public static Vector3? NormalizeToSquareMapSpace(Vector2 normalizedPos)
        {
            Vector3 mapSpacePosition = normalizedPos;
            mapSpacePosition.x = (mapSpacePosition.x * 2f - 1f) * Main.Settings.MapHeight;
            mapSpacePosition.y = (mapSpacePosition.y * 2f - 1f) * Main.Settings.MapWidth;
            mapSpacePosition.z = 0f;

            return mapSpacePosition;
        }
    }
}
