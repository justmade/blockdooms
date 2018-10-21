﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelChunksObjectChunkExplosion))]
    public class VoxelChunksObjectChunkExplosionEditor : EditorCommon
    {
        public VoxelChunksObjectChunkExplosion explosionObject { get; protected set; }
        public VoxelChunksObjectChunkExplosionCore explosionObjectCore { get; protected set; }

        public VoxelChunksObjectChunk chunkObject { get; protected set; }
        public VoxelChunksObjectChunkCore chunkCore { get; protected set; }

        public VoxelChunksObject voxelObject { get; protected set; }
        public VoxelChunksObjectExplosion voxelExplosionObject { get; protected set; }
        public VoxelChunksObjectExplosionCore voxelExplosionCore { get; protected set; }

        #region GuiStyle
        private GUIStyle guiStyleMagentaBold;
        private GUIStyle guiStyleRedBold;
        private GUIStyle guiStyleFoldoutBold;
        #endregion

        protected void OnEnable()
        {
            explosionObject = target as VoxelChunksObjectChunkExplosion;
            if (explosionObject == null) return;
            explosionObjectCore = new VoxelChunksObjectChunkExplosionCore(explosionObject);
            chunkObject = explosionObject.GetComponent<VoxelChunksObjectChunk>();
            if (chunkObject == null) return;
            chunkCore = new VoxelChunksObjectChunkCore(chunkObject);
            if (explosionObject.transform.parent == null) return;
            voxelObject = explosionObject.transform.parent.GetComponent<VoxelChunksObject>();
            if (voxelObject == null) return;
            voxelExplosionObject = voxelObject.GetComponent<VoxelChunksObjectExplosion>();
            if (voxelExplosionObject == null) return;
            voxelExplosionCore = new VoxelChunksObjectExplosionCore(voxelExplosionObject);
        }
        protected void OnDisable()
        {
            if (explosionObject == null || chunkObject == null || voxelObject == null) return;
        }

        public override void OnInspectorGUI()
        {
            if (explosionObject == null || chunkObject == null || voxelObject == null)
            {
                DrawDefaultInspector();
                return;
            }
            
            #region GuiStyle
            if (guiStyleMagentaBold == null)
                guiStyleMagentaBold = new GUIStyle(EditorStyles.boldLabel);
            guiStyleMagentaBold.normal.textColor = Color.magenta;
            if (guiStyleRedBold == null)
                guiStyleRedBold = new GUIStyle(EditorStyles.boldLabel);
            guiStyleRedBold.normal.textColor = Color.red;
            if (guiStyleFoldoutBold == null)
                guiStyleFoldoutBold = new GUIStyle(EditorStyles.foldout);
            guiStyleFoldoutBold.fontStyle = FontStyle.Bold;
            #endregion

            serializedObject.Update();

            InspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected void InspectorGUI()
        {
            var prefabType = PrefabUtility.GetPrefabType(voxelObject.gameObject);
            var prefabEnable = prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance;

            #region Object
            {
                explosionObject.edit_objectFoldout = EditorGUILayout.Foldout(explosionObject.edit_objectFoldout, "Object", guiStyleFoldoutBold);
                if (explosionObject.edit_objectFoldout)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    #region Mesh
                    {
                        #region Title
                        {
                            if (explosionObject.meshes == null || explosionObject.meshes.Count == 0)
                                EditorGUILayout.LabelField("Mesh", guiStyleMagentaBold);
                            else if (prefabEnable)
                            {
                                bool contains = true;
                                for (int i = 0; i < explosionObject.meshes.Count; i++)
                                {
                                    if (explosionObject.meshes[i] == null || explosionObject.meshes[i].mesh == null || !AssetDatabase.Contains(explosionObject.meshes[i].mesh))
                                    {
                                        contains = false;
                                        break;
                                    }
                                }
                                EditorGUILayout.LabelField("Mesh", contains ? EditorStyles.boldLabel : guiStyleRedBold);
                            }
                            else
                                EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                        }
                        #endregion
                        EditorGUI.indentLevel++;
                        #region Mesh
                        if (explosionObject.meshes != null)
                        {
                            for (int i = 0; i < explosionObject.meshes.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.ObjectField(explosionObject.meshes[i].mesh, typeof(Mesh), false);
                                    EditorGUI.EndDisabledGroup();
                                }
                                if (explosionObject.meshes[i].mesh != null)
                                {
                                    if (!IsMainAsset(explosionObject.meshes[i].mesh))
                                    {
                                        if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Create Mesh
                                            string path = EditorUtility.SaveFilePanel("Save mesh", chunkCore.GetDefaultPath(), string.Format("{0}_{1}_explosion_mesh{2}.asset", voxelObject.gameObject.name, chunkObject.chunkName, i), "asset");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                if (path.IndexOf(Application.dataPath) < 0)
                                                {
                                                    SaveInsideAssetsFolderDisplayDialog();
                                                }
                                                else
                                                {
                                                    Undo.RecordObject(explosionObject, "Save Mesh");
                                                    path = path.Replace(Application.dataPath, "Assets");
                                                    AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].mesh), path);
                                                    explosionObject.meshes[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                                                    voxelExplosionCore.Generate();
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    {
                                        if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Reset Mesh
                                            Undo.RecordObject(voxelExplosionObject, "Reset Mesh");
                                            Undo.RecordObjects(voxelExplosionObject.chunksExplosion, "Reset Mesh");
                                            explosionObject.meshes[i].mesh = null;
                                            voxelExplosionCore.Generate();
                                            #endregion
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        #endregion
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    #region Material
                    if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        {
                            if (explosionObject.materials == null || explosionObject.materials.Count == 0)
                                EditorGUILayout.LabelField("Material", guiStyleMagentaBold);
                            else if (prefabEnable)
                            {
                                bool contains = true;
                                for (int i = 0; i < explosionObject.materials.Count; i++)
                                {
                                    if (!AssetDatabase.Contains(explosionObject.materials[i]))
                                    {
                                        contains = false;
                                        break;
                                    }
                                }
                                EditorGUILayout.LabelField("Material", contains ? EditorStyles.boldLabel : guiStyleRedBold);
                            }
                            else
                                EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                        }
                        EditorGUI.indentLevel++;
                        #region Material
                        for (int i = 0; i < explosionObject.materials.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.ObjectField(explosionObject.materials[i], typeof(Material), false);
                                EditorGUI.EndDisabledGroup();
                            }
                            if (explosionObject.materials[i] != null)
                            {
                                if (!IsMainAsset(explosionObject.materials[i]))
                                {
                                    if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                    {
                                        #region Create Material
                                        string defaultName = string.Format("{0}_{1}_explosion_mat{2}.mat", voxelObject.gameObject.name, chunkObject.chunkName, i);
                                        string path = EditorUtility.SaveFilePanel("Save material", chunkCore.GetDefaultPath(), defaultName, "mat");
                                        if (!string.IsNullOrEmpty(path))
                                        {
                                            if (path.IndexOf(Application.dataPath) < 0)
                                            {
                                                SaveInsideAssetsFolderDisplayDialog();
                                            }
                                            else
                                            {
                                                Undo.RecordObject(explosionObject, "Save Material");
                                                path = path.Replace(Application.dataPath, "Assets");
                                                AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.materials[i]), path);
                                                explosionObject.materials[i] = AssetDatabase.LoadAssetAtPath<Material>(path);
                                                voxelExplosionCore.Generate();
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                {
                                    if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                    {
                                        #region Reset Material
                                        Undo.RecordObject(voxelExplosionObject, "Reset Material");
                                        Undo.RecordObjects(voxelExplosionObject.chunksExplosion, "Reset Material");
                                        if (!IsMainAsset(explosionObject.materials[i]))
                                            explosionObject.materials[i] = null;
                                        else
                                            explosionObject.materials[i] = Instantiate<Material>(explosionObject.materials[i]);
                                        voxelExplosionCore.Generate();
                                        #endregion
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        #endregion
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    EditorGUILayout.EndVertical();
                }
            }
            #endregion

            #region Generate
            {
                if (GUILayout.Button("Generate"))
                {
                    Undo.RecordObject(voxelExplosionObject, "Generate Voxel Explosion");
                    Undo.RecordObjects(voxelExplosionObject.chunksExplosion, "Generate Voxel Explosion");
                    voxelExplosionCore.Generate();
                }
            }
            #endregion
        }
    }
}

