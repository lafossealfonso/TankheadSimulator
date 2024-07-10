using UnityEngine;


namespace AmazingAssets.WireframeShader
{

    [ExecuteAlways]
    public class WireframeMaskController : MonoBehaviour
    {
        public enum MaskType { Plane, Sphere }


        public MaskType maskType;
        public Transform maskObject;
        public Material[] materials;


        void Update()
        {
            if (maskObject != null && materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null)
                        continue;


                    switch (maskType)
                    {
                        case MaskType.Plane:
                            materials[i].SetVector("_WireframeShaderMaskPlanePosition", maskObject.position);
                            materials[i].SetVector("_WireframeShaderMaskPlaneNormal", maskObject.up);
                            break;

                        case MaskType.Sphere:
                            materials[i].SetVector("_WireframeShaderMaskSpherePosition", maskObject.position);

                            float radius = Mathf.Abs(maskObject.localScale.x) * 0.5f;
                            materials[i].SetFloat("_WireframeShaderMaskSphereRadius", radius);
                            break;


                        //case MaskType.Box:
                        //    break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}