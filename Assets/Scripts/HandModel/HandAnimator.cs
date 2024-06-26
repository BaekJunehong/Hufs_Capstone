using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using MediaPipe.HandPose;
using System;
using System.Collections.Generic;

public sealed class HandAnimator : MonoBehaviour
{
    #region Public method

    public static HandAnimator instance;

    public Vector3 GetPoint(int index)
      => _pipeline.GetKeyPoint(index);

    public void OnRenderHand()
    {
      var layer = gameObject.layer;

      // Joint balls
      for (var i = 0; i < HandPipeline.KeyPointCount; i++)
      {
          var xform = CalculateJointXform(_pipeline.GetKeyPoint(i));
          Graphics.DrawMesh(_jointMesh, xform, _jointMaterial, layer);
      }

        // Bones
      foreach (var pair in BonePairs)
      {
          var p1 = _pipeline.GetKeyPoint(pair.Item1);
          var p2 = _pipeline.GetKeyPoint(pair.Item2);
          var xform = CalculateBoneXform(p1, p2);
          Graphics.DrawMesh(_boneMesh, xform, _boneMaterial, layer);
      }

    }

    public float GetScore
      => _pipeline.Score;

    public float GetHandedness
      => _pipeline.Handedness;

    #endregion

    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [Space]
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] bool _useAsyncReadback = true;
    [Space]
    [SerializeField] Mesh _jointMesh = null;
    [SerializeField] Mesh _boneMesh = null;
    [Space]
    [SerializeField] Material _jointMaterial = null;
    [SerializeField] Material _boneMaterial = null;
    [Space]
    [SerializeField] RawImage _monitorUI = null;

    #endregion

    #region Private members

    HandPipeline _pipeline;

    static readonly (int, int)[] BonePairs =
    {
        (0, 1), (1, 2), (1, 2), (2, 3), (3, 4),     // Thumb
        (5, 6), (6, 7), (7, 8),                     // Index finger
        (9, 10), (10, 11), (11, 12),                // Middle finger
        (13, 14), (14, 15), (15, 16),               // Ring finger
        (17, 18), (18, 19), (19, 20),               // Pinky
        (0, 17), (2, 5), (5, 9), (9, 13), (13, 17)  // Palm
    };

    Matrix4x4 CalculateJointXform(Vector3 bpos)
    {
      Vector3 pos = bpos + new Vector3 (12, 0, 0);
      return Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * 0.04f);
    }

    Matrix4x4 CalculateBoneXform(Vector3 bp1, Vector3 bp2)
    {
        Vector3 p1 = bp1 + new Vector3 (12, 0, 0);
        Vector3 p2 = bp2 + new Vector3 (12, 0, 0);
        var length = Vector3.Distance(p1, p2) / 2;
        var radius = 0.015f;

        var center = (p1 + p2) / 2;
        var rotation = Quaternion.FromToRotation(Vector3.up, p2 - p1);
        var scale = new Vector3(radius, length, radius);

        return Matrix4x4.TRS(center, rotation, scale);
    }

    #endregion

    #region MonoBehaviour implementation

    private void Awake()
    {
        if(HandAnimator.instance == null)
        {
          HandAnimator.instance = this;
        }
    }

    void Start()
      => _pipeline = new HandPipeline(_resources);
    
    void OnDestroy()
      => _pipeline.Dispose();

    void LateUpdate()
    {
      
        // Feed the input image to the Hand pose pipeline.
        _pipeline.UseAsyncReadback = _useAsyncReadback;
        _pipeline.ProcessImage(_source.Texture);

        // UI update
        if(_monitorUI == null) return;
        _monitorUI.texture = _source.Texture;
    }

    #endregion
}
