using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Microsoft.Kinect;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using Coding4Fun.Kinect.Wpf;

namespace UI_SwayDetection
{
    /// <summary>
    /// Serializes a Kinect skeleton to JSON fromat.
    /// </summary>
    public static class SkeletonSerializer
    {
        [DataContract]
        class JSONSkeletonCollection
        {
            [DataMember(Name = "skeletons")]
            public List<JSONSkeleton> Skeletons { get; set; }
        }

        [DataContract]
        class JSONSkeleton
        {
            [DataMember(Name = "id")]
            public string ID { get; set; }

            [DataMember(Name = "joints")]
            public List<JSONJoint> Joints { get; set; }
        }

        [DataContract]
        class JSONJoint
        {
            [DataMember(Name = "Label")]
            public string Label { get; set; }

            [DataMember(Name = "x")]
            public float X { get; set; }

            [DataMember(Name = "y")]
            public float Y { get; set; }

            [DataMember(Name = "z")]
            public float Z { get; set; }

        }

        public static string Serialize(this List<Skeleton> skeletons)
        {
            JSONSkeletonCollection jsonSkeletons = new JSONSkeletonCollection { Skeletons = new List<JSONSkeleton>() };

            foreach (var skeleton in skeletons)
            {
                JSONSkeleton jsonSkeleton = new JSONSkeleton
                {
                    ID = skeleton.TrackingId.ToString(),
                    Joints = new List<JSONJoint>()
                };

                foreach (Joint joint in skeleton.Joints)
                {

                    //Joint scaled = joint.ScaleTo(1000, 1000);

                    jsonSkeleton.Joints.Add(new JSONJoint
                    {
                        Label = joint.JointType.ToString(),
                        X = joint.Position.X,
                        Y = joint.Position.Y,
                        Z = joint.Position.Z
                    });
                   
                }

                jsonSkeletons.Skeletons.Add(jsonSkeleton);
            }

            return Serialize(jsonSkeletons);
        }
        /* Added by Tatiana Alexenko */
        public static List<Skeleton> Deserialize(string json)
        {
            List<Skeleton> skellist = new List<Skeleton>();
            JSONSkeletonCollection results;
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializerSettings settings =
                        new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;

                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(JSONSkeletonCollection), settings);

                results = (JSONSkeletonCollection)serializer.ReadObject(ms);
                
            }
            foreach (var skeleton in results.Skeletons)
            {
                Skeleton kskel = new Skeleton();
                {
                    kskel.TrackingId = Int32.Parse(skeleton.ID);
                    kskel.TrackingState = (SkeletonTrackingState)Enum.Parse(typeof(SkeletonTrackingState), "Tracked");
                    //Joints = new List<JSONJoint>()
                };

                foreach (var joint in skeleton.Joints)
                {
                    JointType jtype = (JointType)Enum.Parse(typeof(JointType), joint.Label);
                    //Joint scaled = joint.ScaleTo(1000, 1000);
                    SkeletonPoint sp = new SkeletonPoint();
                    sp.X = joint.X;
                    sp.Y=joint.Y;
                    sp.Z=joint.Z;
                    JointTrackingState js;
                    js=(JointTrackingState)Enum.Parse(typeof(JointTrackingState), "Tracked");
                    Joint jres=kskel.Joints[jtype];
                    jres.TrackingState=js;
                    jres.Position=sp;
                    kskel.Joints[jres.JointType]=jres;
                   
                }

                skellist.Add(kskel);
            }

            return skellist;
        }
        /* End of Tatiana Alexenko code*/
        // Resource: http://pietschsoft.com/post/2008/02/NET-35-JSON-Serialization-using-the-DataContractJsonSerializer.aspx.
        private static string Serialize(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();

            return retVal;
        }
    }
}
