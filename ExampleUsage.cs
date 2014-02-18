/* This is not meant to be compiled directly, just an example of usage */

private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            List<Skeleton> skels = new List<Skeleton>(); //serialization/deseraialization done on a list of skeletons
            /* Kinect will track multiple skeletons by default. It was easier to stick with a list even though we only 
            care about 1 skeleton in this example*/

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (skeletons == null)
                    {
                        skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    skeletonFrame.CopySkeletonDataTo(skeletons);
              
            /* Above is clunky, but no direct way to go from skeletonFrame to list To My Knowledge*/
            //add skeletons to list
            foreach (Skeleton s in skeletons)
            {
                if (s.TrackingState == SkeletonTrackingState.Tracked)
                {
                    skels.Add(s);
                }
            }
            /***************************SERIALIZATION ***************************************/
            string json;  //put the skeleton string here
            List<Skeleton> skels_d = new List<Skeleton>(); //will be deserializing to this list later
            if (skels.Count > 0)
            {
                json = skels.Serialize(); // json is now the list of skeletons in JSON format as a string
                System.Diagnostics.Debug.WriteLine(json); //in case you don't believe me
                
                /****************** DESERIALIZE *******************************************/
                skels_d = SkeletonSerializer.Deserialize(json);
                
                /**************** IF YOU'RE NOT THE TRUSTING TYPE *********************/
                foreach (Skeleton sk in skels_d){
                    System.Diagnostics.Debug.WriteLine(sk.TrackingState);
                    foreach (Joint j in sk.Joints){
                        System.Diagnostics.Debug.WriteLine(j.Position.X);
                    }
                }**********************************************************************/
            }
            
            //using the DESEREALIZED Skeleton now
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length!=0)
                {
                    foreach (Skeleton skel in skels_d)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                            this.SwayDetection(skel);
                            this.DrawShape(skel);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
                }
            }
        }
