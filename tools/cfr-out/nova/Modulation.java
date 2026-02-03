/*
 * Decompiled with CFR 0.152.
 */
package nova;

import javax.swing.Box;
import nova.CopyBlock;
import nova.EmptyRow;
import nova.Nibble;
import nova.Patch;
import nova.Row;

public class Modulation
extends CopyBlock {
    Nibble type;
    Nibble speed;
    Nibble depth;
    Nibble tempo;
    Nibble hicut;
    Nibble feedback;
    Nibble fb_hicut;
    Nibble delay;
    Nibble width;
    Nibble typeT;
    Nibble range;
    Nibble mix;
    Nibble onoff;

    public Modulation(Patch patch) {
        super("Modulation", patch, true);
        this.activeNibbles = new int[]{56, 41, 42, 43, 44, 45, 46, 47, 48, 51, 52, 53, 54};
        try {
            for (int i = 0; i < this.activeNibbles.length; ++i) {
                this.nibbles.add(this.parent.getNibble(this.activeNibbles[i]));
            }
            this.onoff = this.parent.getNibble(56);
            this.type = this.parent.getNibble(41);
            this.speed = this.parent.getNibble(42);
            this.depth = this.parent.getNibble(43);
            this.tempo = this.parent.getNibble(44);
            this.hicut = this.parent.getNibble(45);
            this.feedback = this.parent.getNibble(46);
            this.fb_hicut = this.parent.getNibble(47);
            this.delay = this.parent.getNibble(48);
            this.width = this.parent.getNibble(51);
            this.typeT = this.parent.getNibble(52);
            this.range = this.parent.getNibble(53);
            this.mix = this.parent.getNibble(54);
            this.type.updateNibble("Type", 54);
            this.onoff.updateNibble("On", 2);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Modulation Block");
        }
        this.refresh();
    }

    void refresh() {
        int n = this.type.getValue();
        int n2 = 0;
        this.box.removeAll();
        for (int i = 2; i < this.activeNibbles.length; ++i) {
            Nibble nibble = this.parent.getNibble(this.activeNibbles[i]);
            nibble.setType(100);
        }
        this.box.add(new Row("Modulation", this.onoff, n2++));
        this.box.add(new Row("Modulation", this.type, n2++));
        switch (n) {
            case 0: {
                this.activeNibbles = new int[]{56, 41, 42, 44, 43, 45, 48, 54};
                this.speed.updateNibble("Speed (Hz)", 20);
                this.tempo.updateNibble("Tempo", 21);
                this.depth.updateNibble("Depth (%)", 116);
                this.hicut.updateNibble("Hi Cut (Hz)", 22);
                this.delay.updateNibble("Delay (ms)", 90);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.speed, n2++));
                this.box.add(new Row("", this.tempo, n2++));
                this.box.add(new Row("", this.depth, n2++));
                this.box.add(new Row("", this.hicut, n2++));
                this.box.add(new Row("", this.delay, n2++));
                this.box.add(new Row("", this.mix, n2++));
                break;
            }
            case 1: {
                this.activeNibbles = new int[]{56, 41, 42, 44, 43, 45, 46, 47, 48, 54};
                this.speed.updateNibble("Speed (Hz)", 20);
                this.tempo.updateNibble("Tempo", 21);
                this.depth.updateNibble("Depth (%)", 116);
                this.hicut.updateNibble("Hi Cut (Hz)", 22);
                this.feedback.updateNibble("Feedback (%)", 103);
                this.fb_hicut.updateNibble("Feedback HiCut (Hz)", 22);
                this.delay.updateNibble("Delay (ms)", 90);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.speed, n2++));
                this.box.add(new Row("", this.tempo, n2++));
                this.box.add(new Row("", this.depth, n2++));
                this.box.add(new Row("", this.hicut, n2++));
                this.box.add(new Row("", this.feedback, n2++));
                this.box.add(new Row("", this.fb_hicut, n2++));
                this.box.add(new Row("", this.delay, n2++));
                this.box.add(new Row("", this.mix, n2++));
                break;
            }
            case 2: {
                this.activeNibbles = new int[]{56, 41, 42, 44, 43, 45};
                this.speed.updateNibble("Speed (Hz)", 20);
                this.tempo.updateNibble("Tempo", 21);
                this.depth.updateNibble("Depth (%)", 116);
                this.hicut.updateNibble("Hi Cut (Hz)", 22);
                this.box.add(new Row("", this.speed, n2++));
                this.box.add(new Row("", this.tempo, n2++));
                this.box.add(new Row("", this.depth, n2++));
                this.box.add(new Row("", this.hicut, n2++));
                break;
            }
            case 3: {
                this.activeNibbles = new int[]{56, 41, 42, 44, 43, 53, 46, 54};
                this.speed.updateNibble("Speed (Hz)", 20);
                this.tempo.updateNibble("Tempo", 21);
                this.depth.updateNibble("Depth (%)", 116);
                this.range.updateNibble("Range", 28);
                this.feedback.updateNibble("Feedback (%)", 103);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.speed, n2++));
                this.box.add(new Row("", this.tempo, n2++));
                this.box.add(new Row("", this.depth, n2++));
                this.box.add(new Row("", this.range, n2++));
                this.box.add(new Row("", this.feedback, n2++));
                this.box.add(new Row("", this.mix, n2++));
                break;
            }
            case 4: {
                this.activeNibbles = new int[]{56, 41, 42, 44, 43, 52, 51, 45};
                this.speed.updateNibble("Speed (Hz)", 20);
                this.tempo.updateNibble("Tempo", 21);
                this.depth.updateNibble("Depth (%)", 116);
                this.typeT.updateNibble("Type", 27);
                this.width.updateNibble("Width (%)", 116);
                this.hicut.updateNibble("Hi Cut (Hz)", 22);
                this.box.add(new Row("", this.speed, n2++));
                this.box.add(new Row("", this.tempo, n2++));
                this.box.add(new Row("", this.depth, n2++));
                this.box.add(new Row("", this.typeT, n2++));
                this.box.add(new Row("", this.width, n2++));
                this.box.add(new Row("", this.hicut, n2++));
                break;
            }
            case 5: {
                this.activeNibbles = new int[]{56, 41, 42, 44, 43};
                this.speed.updateNibble("Speed (Hz)", 20);
                this.tempo.updateNibble("Tempo", 21);
                this.depth.updateNibble("Depth (%)", 116);
                this.box.add(new Row("", this.speed, n2++));
                this.box.add(new Row("", this.tempo, n2++));
                this.box.add(new Row("", this.depth, n2++));
            }
        }
        while (n2 < 14) {
            this.box.add(new EmptyRow(true, n2++));
        }
    }

    int getBlockType(Box box) {
        return 3;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
