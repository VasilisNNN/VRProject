using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Platform.Models;
using Meta.XR.MRUtilityKit;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Threading;

public class RequestScan : MonoBehaviour
{
    public UnityEvent OnSceneLoaded { get; private set; }
    private MRUKRoom currentRoom;
    private bool MeshIsCreated;
    XRMeshSubsystem meshSubsystem;
    public Material roomMaterial;
    /*void Start()
    {
      
         Core.Initialize();
         Permission.RequestUserPermission("com.oculus.permission.USE_SCENE");
         // MRUK.Instance.RegisterSceneLoadedCallback += OnSceneLoaded;

         // Get the XR Mesh Subsystem
         List<XRMeshSubsystem> meshSubsystems = new List<XRMeshSubsystem>();
         SubsystemManager.GetInstances(meshSubsystems);
         if (meshSubsystems.Count > 0)
             meshSubsystem = meshSubsystems[0];

         // Start scanning
         if (meshSubsystem != null)
             meshSubsystem.meshDensity =1.0f; // max density

         MRUK.Instance.SceneLoadedEvent.AddListener(SceneLoadedEvent);
         
    }*/



    IEnumerator Start()
    {

        if (InitializeOnAwake.pl == null) yield return null;
        float timer = 0f;
        float timer2 = 0f;
        // wait for MRUK system
        while ((MRUK.Instance == null || !MRUK.Instance.IsInitialized) && timer < 10)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        // load scene from device
        if (OVRPermissionsRequester.ScenePermission!="" )
        yield return MRUK.Instance.LoadSceneFromDevice();

        // wait until room is ready
        MRUKRoom room = null;
        while ((room = MRUK.Instance.GetCurrentRoom()) == null && timer2 < 10)
        {
            timer += Time.deltaTime; 
            yield return null;
        }
        Debug.Log("Room Loaded");

        // spawn visuals
        SpawnRoom(room);

        MeshIsCreated = true;
    }

    void SpawnRoom(MRUKRoom room)
    {
        foreach (var anchor in room.Anchors)
        {
            // DEBUG INFO
            Debug.Log($"{anchor.Label} | Plane: {anchor.PlaneRect.HasValue} | Volume: {anchor.VolumeBounds.HasValue}");

            // ✅ HANDLE PLANES (walls, floor, ceiling)
            if (anchor.PlaneRect.HasValue)
            {
                var rect = anchor.PlaneRect.Value;

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
                plane.name = anchor.Label.ToString();
           
                plane.transform.SetPositionAndRotation(
                    anchor.transform.position,
                    anchor.transform.rotation
                );

                plane.transform.localScale = new Vector3(
                    rect.x,
                    rect.y,
                    1f
                );

                ApplyMaterial(plane);
            }
            // ✅ HANDLE VOLUMES (tables, couches, etc.)
            else if (anchor.VolumeBounds.HasValue)
            {
                var bounds = anchor.VolumeBounds.Value;

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = anchor.Label.ToString();

                cube.transform.SetPositionAndRotation(
                    anchor.transform.position,
                    anchor.transform.rotation
                );

                cube.transform.localScale = bounds.size;

                ApplyMaterial(cube);
            }
        }
    }

    void ApplyMaterial(GameObject obj)
    {
        var renderer = obj.GetComponent<MeshRenderer>();

        if (roomMaterial != null)
        {
            renderer.material = roomMaterial;
        }
        else
        {
            renderer.material = new Material(Shader.Find("Standard"));
        }
    }


    private void SceneLoadedEvent()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        if (room == null || room.RoomMeshData == null) return;
        
        transform.position = room.transform.position;
        transform.rotation = room.transform.rotation;
        var mesh = CreateMeshFromRoomMeshData(room.RoomMeshData.Value);
        
        if (room.RoomMeshData == null || mesh == null) return;

        var roomMesh = room.RoomMeshData.Value;
            // Add MeshFilter if it doesn't exist
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            // Add MeshRenderer if it doesn't exist
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            // Create materials for each submesh
            int submeshCount = mesh.subMeshCount;
            Material[] materials = new Material[submeshCount];


            // Create a material for each submesh with different colors to distinguish them
            for (int i = 0; i < submeshCount; i++)
            {
                // Create a local color variable
                Color color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // Default gray color

                // Assign different colors to different semantic types if available
                if (i < roomMesh.Faces.Count)
                {
                    var semanticLabel = roomMesh.Faces[i].SemanticLabel;

                    // Assign colors based on semantic label
                    switch (semanticLabel)
                    {
                        case MRUKAnchor.SceneLabels.FLOOR:
                            color = new Color(0.2f, 0.6f, 0.2f, 1.0f); // Green for floor
                            break;
                        case MRUKAnchor.SceneLabels.CEILING:
                            color = new Color(0.8f, 0.8f, 0.8f, 1.0f); // White for ceiling
                            break;
                        case MRUKAnchor.SceneLabels.WALL_FACE:
                            color = new Color(0.6f, 0.6f, 0.8f, 1.0f); // Blue for walls
                            break;
                        case MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE:
                            color = new Color(0.8f, 0.3f, 0.8f, 1.0f); // Purple for invisible walls
                            break;
                        case MRUKAnchor.SceneLabels.INNER_WALL_FACE:
                            color = new Color(0.4f, 0.4f, 0.6f, 1.0f); // Darker blue for inner walls
                            break;
                        case MRUKAnchor.SceneLabels.WINDOW_FRAME:
                            color = new Color(0.7f, 0.9f, 1.0f, 1.0f); // Light blue for windows
                            break;
                        case MRUKAnchor.SceneLabels.DOOR_FRAME:
                            color = new Color(0.6f, 0.4f, 0.2f, 1.0f); // Brown for doors
                            break;
                    }
                }

                // Create the material
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                materials[i] = new Material(shader)
                {
                    color = color
                };
            }

            // Assign the materials to the renderer
            meshRenderer.materials = materials;

            // Assign the mesh to the MeshFilter
            meshFilter.mesh = mesh;
        
        
    }

    private static Mesh CreateMeshFromRoomMeshData(MRUKRoom.RoomMesh roomMesh)
    {
        // Find the average position of all vertices
        Vector3 center = Vector3.zero;
        foreach (var vertex in roomMesh.Vertices)
        {
            center += vertex;
        }
        center /= roomMesh.Vertices.Count;

        Vector3[] scaledVertices = new Vector3[roomMesh.Vertices.Count];
        for (int i = 0; i < roomMesh.Vertices.Count; i++)
        {
            // Scale each vertex relative to the center
            const float scaleFactor = 1.001f;
            scaledVertices[i] = center + (roomMesh.Vertices[i] - center) * scaleFactor;
        }

        Mesh mesh = new Mesh
        {
            vertices = scaledVertices,
            subMeshCount = roomMesh.Faces.Count
        };

        // Create submeshes for each face
        for (int i = 0; i < roomMesh.Faces.Count; i++)
        {
            // Set triangles for this submesh
            mesh.SetTriangles(roomMesh.Faces[i].Indices.ToArray(), i);
        }

        return mesh;
    }

    IEnumerator Init()
    {
        if (MeshIsCreated) yield return null;

        while (MRUK.Instance == null || !MRUK.Instance.IsInitialized)
            yield return null;

        
        currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null)
        {
            Debug.LogError("No room found");
            yield break;
        }

        // Loop through anchors and show mesh
        foreach (var anchor in currentRoom.Anchors)
        {
            var mf = anchor.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // Ensure it’s visible
                var mr = anchor.GetComponent<MeshRenderer>();
                if (mr == null)
                {
                    mr = anchor.gameObject.AddComponent<MeshRenderer>();
                    mr.material = new Material(Shader.Find("Standard"));
                }

                // Optional: add collider if needed
                var col = anchor.GetComponent<MeshCollider>();
                if (col == null)
                {
                    col = anchor.gameObject.AddComponent<MeshCollider>();
                    col.sharedMesh = mf.sharedMesh;
                }
                MeshIsCreated = true;
                Debug.Log("Displayed mesh: " + anchor.name);
            }
        }
    }

   
}
