﻿using BabylonExport.Entities;
using GLTFExport.Entities;

namespace Maya2Babylon
{
    partial class BabylonExporter
    {
        private GLTFNode ExportAbstractMesh(BabylonAbstractMesh babylonAbstractMesh, GLTF gltf, GLTFNode gltfParentNode, BabylonScene babylonScene)
        {
            RaiseMessage("GLTFExporter.AbstractMesh | Export abstract mesh named: " + babylonAbstractMesh.name, 1);

            // Node
            var gltfNode = new GLTFNode();
            gltfNode.name = babylonAbstractMesh.name;
            gltfNode.index = gltf.NodesList.Count;
            gltf.NodesList.Add(gltfNode);

            // Hierarchy
            if (gltfParentNode != null)
            {
                RaiseMessage("GLTFExporter.AbstractMesh | Add " + babylonAbstractMesh.name + " as child to " + gltfParentNode.name, 2);
                gltfParentNode.ChildrenList.Add(gltfNode.index);
                gltfNode.parent = gltfParentNode;
            }
            else
            {
                // It's a root node
                // Only root nodes are listed in a gltf scene
                RaiseMessage("GLTFExporter.AbstractMesh | Add " + babylonAbstractMesh.name + " as root node to scene", 2);
                gltf.scenes[0].NodesList.Add(gltfNode.index);
            }

            // Transform
            gltfNode.translation = babylonAbstractMesh.position;
            if (babylonAbstractMesh.rotationQuaternion != null)
            {
                gltfNode.rotation = babylonAbstractMesh.rotationQuaternion;
            }
            else
            {
                // Convert rotation vector to quaternion
                BabylonVector3 rotationVector3 = new BabylonVector3
                {
                    X = babylonAbstractMesh.rotation[0],
                    Y = babylonAbstractMesh.rotation[1],
                    Z = babylonAbstractMesh.rotation[2]
                };
                gltfNode.rotation = rotationVector3.toQuaternion().ToArray();
            }
            gltfNode.scale = babylonAbstractMesh.scaling;

            // Switch coordinate system at object level
            gltfNode.translation[2] *= -1;
            gltfNode.rotation[0] *= -1;
            gltfNode.rotation[1] *= -1;

            // Mesh
            var gltfMesh = gltf.MeshesList.Find(_gltfMesh => _gltfMesh.idGroupInstance == babylonAbstractMesh.idGroupInstance);
            if (gltfMesh != null)
            {
                gltfNode.mesh = gltfMesh.index;

                // Skin
                if (gltfMesh.idBabylonSkeleton.HasValue)
                {
                    // TODO - Skin
                    //var babylonSkeleton = babylonScene.skeletons[gltfMesh.idBabylonSkeleton.Value];
                    //// Export a new skeleton if necessary and a new skin
                    //var gltfSkin = ExportSkin(babylonSkeleton, gltf, gltfNode);
                    //gltfNode.skin = gltfSkin.index;
                }
            }

            // TODO - Animations
            //// Animations
            //ExportNodeAnimation(babylonAbstractMesh, gltf, gltfNode, babylonScene);

            return gltfNode;
        }
    }
}
