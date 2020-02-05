using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PQScoreboard
{
    public class TripleBuffer
    {
        private Stack<SingleBuffer> freeBuffers;
        private SingleBuffer[] buffers;
        private object mutex;
        private SingleBuffer nextDrawingBuffer;

        public TripleBuffer(int width, int height, bool textAntialiasing)
        {
            mutex = new object();
            freeBuffers = new Stack<SingleBuffer>(3);
            buffers = new SingleBuffer[3];
            nextDrawingBuffer = null;

            for (int i = 2; i >= 0; --i)
            {
                buffers[i] = new SingleBuffer(width, height, textAntialiasing);
                freeBuffers.Push(buffers[i]);
            }
        }

        public void Dispose()
        {
            if (buffers != null)
            {
                for (int i = 2; i >= 0; --i)
                {
                    buffers[i]?.Dispose();
                }
            }
        }

        public SingleBuffer GetForDrawing()
        {
            lock (mutex)
            {
                SingleBuffer tmp = nextDrawingBuffer;
                nextDrawingBuffer = null;
                return tmp;
            }
        }

        public void ReleaseForDrawing(SingleBuffer buffer)
        {
            lock (mutex)
            {
                freeBuffers.Push(buffer);
            }
        }

        public SingleBuffer GetForRenderer()
        {
            lock (mutex)
            {
                if (freeBuffers.Count == 0)
                {
                    return null;
                }

                return freeBuffers.Pop();
            }
        }

        public void ReleaseForRenderer(SingleBuffer buffer)
        {
            lock (mutex)
            {
                if (nextDrawingBuffer != null)
                {
                    // drawing is too slow
                    freeBuffers.Push(nextDrawingBuffer);
                }

                nextDrawingBuffer = buffer;
            }
        }
    }

    public class SingleBuffer
    {
        public SingleBuffer(int width, int height, bool textAntialiasing)
        {
            RenderTarget = new Bitmap(width, height);
            RenderTargetGraphics = Graphics.FromImage(RenderTarget);

            if (textAntialiasing)
            {
                RenderTargetGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
        }

        public void Dispose()
        {
            RenderTarget?.Dispose();
            RenderTargetGraphics?.Dispose();
        }

        public Bitmap RenderTarget { get; private set; }

        public Graphics RenderTargetGraphics { get; private set; }
    }
}
