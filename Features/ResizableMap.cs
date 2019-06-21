using BattleTech;
using UnityEngine;

namespace ISM3025.Features
{
    public static class ResizableMap
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


        public static Vector3 ClampCameraPosition(Vector3 cameraPosition, float fov, float zPos)
        {
            var clamped = cameraPosition;
            var xView = GetViewSize(Mathf.Abs(zPos), GetHorizontalFov(fov));
            var yView = GetViewSize(Mathf.Abs(zPos), fov);

            var totalWidth = Main.Settings.MapWidth * 2f + 2f * Main.Settings.MapMargin;
            var totalHeight = Main.Settings.MapHeight * 2f + Main.Settings.MapMargin + Main.Settings.MapBottomMargin;

            var leftBoundary = -Main.Settings.MapWidth - Main.Settings.MapMargin + xView;
            var rightBoundary = Main.Settings.MapWidth + Main.Settings.MapMargin - xView;
            var bottomBoundary = -Main.Settings.MapHeight - Main.Settings.MapBottomMargin + yView;
            var topBoundary = Main.Settings.MapHeight + Main.Settings.MapMargin - yView;

            clamped.x = xView * 2f >= totalWidth ? 0
                : Mathf.Clamp(cameraPosition.x, leftBoundary, rightBoundary);

            clamped.y = yView * 2f >= totalHeight ? 0
                : Mathf.Clamp(cameraPosition.y, bottomBoundary, topBoundary);

            return clamped;
        }

        private static float GetViewSize(float zPos, float fov)
        {
            return zPos * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f);
        }

        private static float GetHorizontalFov(float verticalFov)
        {
            return 2.0f * Mathf.Atan((16f/9f) * Mathf.Tan(verticalFov * Mathf.Deg2Rad / 2.0f)) * Mathf.Rad2Deg;
        }
    }
}
