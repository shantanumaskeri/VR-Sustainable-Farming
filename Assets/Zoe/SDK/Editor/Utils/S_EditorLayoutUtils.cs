using SpatialStories;
﻿using UnityEditor;

public static class S_EditorLayoutUtils
{
    public static void ControllerInputSelectionField(ref S_Controllers _controller, ref S_SingleHandsEnum _hand, ref S_BaseInputTypes _baseInputType, ref S_DirectionTypes _directionType, ref S_StickDirections _stickDirectionType)
    {
        ControllerInputSelectionFieldUtil.RenderControllerInputSelectionField(ref _controller, ref _hand, ref _baseInputType, ref _directionType, ref _stickDirectionType);
    }

    public static void Space(int _numberOfSpaces = 1)
    {
        if (_numberOfSpaces <= 0)
        {
            return;
        }

        for (int spaceIterator = 0; spaceIterator < _numberOfSpaces; spaceIterator++)
        {
            EditorGUILayout.Space();
        }
    }
}
