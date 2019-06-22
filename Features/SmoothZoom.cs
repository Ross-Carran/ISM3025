using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using UnityEngine;

namespace ISM3025.Features
{
    public static class SmoothZoom
    {
        private static float _currentZoom;
        private static float _startZoom;
        private static float _targetZoom;
        private static float _zoomVelocity;

        public static void PreUpdate(StarmapRenderer renderer)
        {
            if (!Main.Settings.UseSmoothZoom)
                return;

            _currentZoom = renderer.ZoomLevel;

            var scroll = -Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f && !LazySingletonBehavior<UIManager>.Instance.DoesRaycastHitUI(UIManagerRootType.UIRoot))
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_startZoom == 0f || Mathf.Abs(_targetZoom - _currentZoom) < 0.01)
                {
                    _startZoom = _currentZoom;
                    _targetZoom = _currentZoom;
                }

                _targetZoom = Mathf.Clamp01(_targetZoom + scroll * 1f);
            }
        }

        public static void PostUpdate(StarmapRenderer renderer)
        {
            if (!Main.Settings.UseSmoothZoom)
                return;

            if (Mathf.Abs(_targetZoom - _currentZoom) > 0.01)
            {
                var smoothedZoom = Mathf.SmoothDamp(_currentZoom, _targetZoom, ref _zoomVelocity, Main.Settings.SmoothZoomTime);
                Traverse.Create(renderer).Field("zoomLevel").SetValue(smoothedZoom);

                var fakeCamera = Traverse.Create(renderer).Field("fakeCamera").GetValue<Camera>();
                var newFov = Mathf.Lerp(Main.Settings.MinFov, Main.Settings.MaxFov, smoothedZoom);
                renderer.starmapCamera.fieldOfView = newFov;
                fakeCamera.fieldOfView = newFov;
            }
        }
    }
}
