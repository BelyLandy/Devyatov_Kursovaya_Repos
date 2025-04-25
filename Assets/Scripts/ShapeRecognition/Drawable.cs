using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Drawable : MonoBehaviour
{
    public event Action<DollarPoint[]> OnDrawFinished;


    [SerializeField] private PenColorSettings _penColorSettings;

    [SerializeField] private static int Pen_Width = 1;


    public delegate void Brush_Function(Vector2 world_position);

    public Brush_Function current_brush;

    public LayerMask Drawing_Layers;

    public bool Reset_Canvas_On_Play = true;
    
    public Color Reset_Colour = new Color(0, 0, 0, 0);
    
    public static Drawable drawable;
    
    public Sprite drawable_sprite;
    public Texture2D drawable_texture;

    Vector2 previous_drag_position;
    Color[] clean_colours_array;
    Color transparent;
    Color32[] cur_colors;
    bool mouse_was_previously_held_down = false;
    bool no_drawing_on_current_drag = false;
    
    public void BrushTemplate(Vector2 world_position)
    {
        Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

        cur_colors = drawable_texture.GetPixels32();
        
        if (previous_drag_position == Vector2.zero)
        {
            MarkPixelsToColour(pixel_pos, Pen_Width, _penColorSettings.currentColor);
        }
        else
        {
            ColourBetween(previous_drag_position, pixel_pos, Pen_Width, _penColorSettings.currentColor);
        }

        ApplyMarkedPixelChanges(drawable_texture, cur_colors);
        
        previous_drag_position = pixel_pos;
    }


    private List<DollarPoint> _drawPoints = new List<DollarPoint>();
    
    public void PenBrush(Vector2 world_point)
    {
        Vector2 pixel_pos = WorldToPixelCoordinates(world_point);


        cur_colors = drawable_texture.GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            MarkPixelsToColour(pixel_pos, Pen_Width, _penColorSettings.currentColor);
        }
        else
        {
            ColourBetween(previous_drag_position, pixel_pos, Pen_Width, _penColorSettings.currentColor);
        }

        _drawPoints.Add(new DollarPoint() { Point = pixel_pos, StrokeIndex = _strokeIndex });

        ApplyMarkedPixelChanges(drawable_texture, cur_colors);

        previous_drag_position = pixel_pos;
    }
    
    public void SetPenBrush()
    {
        current_brush = PenBrush;
    }


    private int _strokeIndex;
    private bool _drawStarted;

    private bool isDone;
    
    void Update()
    {
        bool mouse_held_down = Input.GetMouseButton(0);

        if (mouse_held_down)
        {
            Pen_Width = 1;
        }

        if (Input.GetMouseButtonDown(0))
        {
            AudioController.PlaySFX("SlimeSplat");
        }

        if ((mouse_held_down) && !no_drawing_on_current_drag)
        {
            isDone = true;
            
            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
            if (hit != null && hit.transform != null)
            {
                current_brush(mouse_world_position);
            }

            else
            {
                previous_drag_position = Vector2.zero;
                if (!mouse_was_previously_held_down)
                {
                    no_drawing_on_current_drag = true;
                }
            }
        }
        else if (!mouse_held_down)
        {
            previous_drag_position = Vector2.zero;
            no_drawing_on_current_drag = false;

            if (isDone)
            {
                if (_drawPoints.Count > 0)
                {
                    OnDrawFinished?.Invoke(_drawPoints.ToArray());
                    previous_drag_position = Vector2.zero;
                    no_drawing_on_current_drag = false;
                    ResetCanvas(drawable_texture);
                    //Debug.Log(_strokeIndex);
                    _strokeIndex = 0;
                }
                else
                {
                    Debug.Log("Холст пуст!");
                }

                isDone = false;
            }
        }

        if (Input.GetMouseButtonUp(0) && InRange())
        {
            _strokeIndex++;
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_drawPoints.Count > 0)
            {
                OnDrawFinished?.Invoke(_drawPoints.ToArray());
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
                ResetCanvas(drawable_texture);
                Debug.Log(_strokeIndex);
                _strokeIndex = 0;
            }
            else
            {
                Debug.Log("Холст пуст!");
            }
        }*/

        mouse_was_previously_held_down = mouse_held_down;
    }

    private bool InRange()
    {
        Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if the current mouse position overlaps our image
        Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
        return hit != null;
    }
    
    public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
    {
         float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsToColour(cur_position, width, color);
        }
    }


    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            if (x >= (int)drawable_sprite.rect.width || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen, cur_colors);
            }
        }
    }

    public void MarkPixelToChange(int x, int y, Color color, Color32[] textureColors)
    {
        int array_pos = y * (int)drawable_sprite.rect.width + x;
        
        if (array_pos > textureColors.Length || array_pos < 0)
            return;

        textureColors[array_pos] = color;
    }

    public void ApplyMarkedPixelChanges(Texture2D texture, Color32[] colors)
    {
        texture.SetPixels32(colors);
        texture.Apply(false);
    }

    public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                drawable_texture.SetPixel(x, y, color_of_pen);
            }
        }

        drawable_texture.Apply();
    }


    public Vector2 WorldToPixelCoordinates(Vector2 world_position)
    {
        Vector3 local_pos = transform.InverseTransformPoint(world_position);
        
        float pixelWidth = drawable_sprite.rect.width;
        float pixelHeight = drawable_sprite.rect.height;
        float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;
        
        float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
        float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;
        
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        return pixel_pos;
    }
    
    public void ResetCanvas(Texture2D texture)
    {
        _drawPoints.Clear();
        texture.SetPixels(clean_colours_array);
        texture.Apply();
    }


    void Awake()
    {
        drawable = this;

        current_brush = PenBrush;
        
        clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
        for (int x = 0; x < clean_colours_array.Length; x++)
            clean_colours_array[x] = Reset_Colour;
        
        if (Reset_Canvas_On_Play)
            ResetCanvas(drawable_texture);
    }

    private void Reset()
    {
        clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
        ResetCanvas(drawable_texture);
    }

    public Vector3 PixelToWorldCoordinates(Vector2 pixelPos)
    {
        float pixelWidth = drawable_sprite.rect.width;
        float pixelHeight = drawable_sprite.rect.height;
        
        float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;
        
        float uncenteredX = pixelPos.x - pixelWidth / 2;
        float uncenteredY = pixelPos.y - pixelHeight / 2;

        Vector3 localPos = new Vector3(uncenteredX / unitsToPixels, uncenteredY / unitsToPixels, 0f);
        return transform.TransformPoint(localPos);
    }
}