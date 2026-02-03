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

public class Delay
extends CopyBlock {
    Nibble type;
    Nibble d1;
    Nibble d2;
    Nibble tempo1;
    Nibble tempo2;
    Nibble fb1;
    Nibble fb2;
    Nibble hicut;
    Nibble locut;
    Nibble pan1;
    Nibble pan2;
    Nibble damp;
    Nibble release;
    Nibble mix;
    Nibble onoff;
    Nibble width;
    Nibble clip;
    Nibble offset;
    Nibble sense;

    public Delay(Patch patch) {
        super("Delay", patch, true);
        this.activeNibbles = new int[]{72, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70};
        try {
            for (int i = 0; i < this.activeNibbles.length; ++i) {
                this.nibbles.add(this.parent.getNibble(this.activeNibbles[i]));
            }
            this.onoff = this.parent.getNibble(72);
            this.type = this.parent.getNibble(57);
            this.d1 = this.parent.getNibble(58);
            this.d2 = this.parent.getNibble(59);
            this.tempo1 = this.parent.getNibble(60);
            this.tempo2 = this.parent.getNibble(61);
            this.fb1 = this.parent.getNibble(62);
            this.fb2 = this.parent.getNibble(63);
            this.hicut = this.parent.getNibble(64);
            this.locut = this.parent.getNibble(65);
            this.pan1 = this.parent.getNibble(66);
            this.pan2 = this.parent.getNibble(67);
            this.damp = this.parent.getNibble(68);
            this.release = this.parent.getNibble(69);
            this.mix = this.parent.getNibble(70);
            this.type.updateNibble("Type", 52);
            this.onoff.updateNibble("On", 2);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Delay Block");
        }
        this.width = this.tempo2;
        this.clip = this.fb2;
        this.offset = this.pan1;
        this.sense = this.pan2;
        this.refresh();
    }

    void refresh() {
        int n;
        this.box.removeAll();
        for (n = 2; n < this.activeNibbles.length; ++n) {
            Nibble nibble = this.parent.getNibble(this.activeNibbles[n]);
            nibble.setType(100);
        }
        n = 0;
        this.box.add(new Row("Delay", this.onoff, n++));
        this.box.add(new Row("", this.type, n++));
        switch (this.type.getValue()) {
            case 0: {
                this.activeNibbles = new int[]{72, 57, 58, 60, 62, 64, 65, 70};
                this.d1.updateNibble("Delay (ms)", 119);
                this.tempo1.updateNibble("Tempo", 21);
                this.fb1.updateNibble("Feedback (%)", 116);
                this.hicut.updateNibble("HiCut (Hz)", 22);
                this.locut.updateNibble("LoCut (Hz)", 24);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.d1, n++));
                this.box.add(new Row("", this.tempo1, n++));
                this.box.add(new Row("", this.fb1, n++));
                this.box.add(new Row("", this.hicut, n++));
                this.box.add(new Row("", this.locut, n++));
                this.box.add(new Row("", this.mix, n++));
                break;
            }
            case 1: {
                this.activeNibbles = new int[]{72, 57, 58, 60, 63, 62, 64, 65, 70};
                this.d1.updateNibble("Delay (ms)", 119);
                this.tempo1.updateNibble("Tempo", 21);
                this.clip.updateNibble("Clip (dB)", 124);
                this.fb1.updateNibble("Feedback (%)", 116);
                this.hicut.updateNibble("HiCut (Hz)", 22);
                this.locut.updateNibble("LoCut (Hz)", 24);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.d1, n++));
                this.box.add(new Row("", this.tempo1, n++));
                this.box.add(new Row("", this.clip, n++));
                this.box.add(new Row("", this.fb1, n++));
                this.box.add(new Row("", this.hicut, n++));
                this.box.add(new Row("", this.locut, n++));
                this.box.add(new Row("", this.mix, n++));
                break;
            }
            case 2: {
                this.activeNibbles = new int[]{72, 57, 58, 60, 63, 62, 64, 65, 70};
                this.d1.updateNibble("Delay (ms)", 119);
                this.tempo1.updateNibble("Tempo", 21);
                this.clip.updateNibble("Clip (dB)", 124);
                this.fb1.updateNibble("Feedback (%)", 116);
                this.hicut.updateNibble("HiCut (Hz)", 22);
                this.locut.updateNibble("LoCut (Hz)", 24);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.d1, n++));
                this.box.add(new Row("", this.tempo1, n++));
                this.box.add(new Row("", this.clip, n++));
                this.box.add(new Row("", this.fb1, n++));
                this.box.add(new Row("", this.hicut, n++));
                this.box.add(new Row("", this.locut, n++));
                this.box.add(new Row("", this.mix, n++));
                break;
            }
            case 3: {
                this.activeNibbles = new int[]{72, 57, 58, 60, 62, 64, 65, 66, 67, 68, 69, 70};
                this.d1.updateNibble("Delay (ms)", 119);
                this.tempo1.updateNibble("Tempo", 21);
                this.fb1.updateNibble("Feedback (%)", 116);
                this.hicut.updateNibble("HiCut (Hz)", 22);
                this.locut.updateNibble("LoCut (Hz)", 24);
                this.offset.updateNibble("Offset (ms)", 118);
                this.sense.updateNibble("Sense (dB)", 106);
                this.damp.updateNibble("Damp (dB)", 116);
                this.release.updateNibble("Release (ms)", 12);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.d1, n++));
                this.box.add(new Row("", this.tempo1, n++));
                this.box.add(new Row("", this.fb1, n++));
                this.box.add(new Row("", this.hicut, n++));
                this.box.add(new Row("", this.locut, n++));
                this.box.add(new Row("", this.offset, n++));
                this.box.add(new Row("", this.sense, n++));
                this.box.add(new Row("", this.damp, n++));
                this.box.add(new Row("", this.release, n++));
                this.box.add(new Row("", this.mix, n++));
                break;
            }
            case 4: {
                this.activeNibbles = new int[]{72, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 70};
                this.d1.updateNibble("Delay 1 (ms)", 119);
                this.d2.updateNibble("Delay 2 (ms)", 119);
                this.tempo1.updateNibble("Tempo 1", 21);
                this.tempo2.updateNibble("Tempo 2", 21);
                this.fb1.updateNibble("Feedback 1 (%)", 117);
                this.fb2.updateNibble("Feedback 2 (%)", 117);
                this.hicut.updateNibble("HiCut (Hz)", 22);
                this.locut.updateNibble("LoCut (Hz)", 24);
                this.pan1.updateNibble("Pan 1", 107);
                this.pan2.updateNibble("Pan 2", 107);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.d1, n++));
                this.box.add(new Row("", this.d2, n++));
                this.box.add(new Row("", this.tempo1, n++));
                this.box.add(new Row("", this.tempo2, n++));
                this.box.add(new Row("", this.fb1, n++));
                this.box.add(new Row("", this.fb2, n++));
                this.box.add(new Row("", this.hicut, n++));
                this.box.add(new Row("", this.locut, n++));
                this.box.add(new Row("", this.pan1, n++));
                this.box.add(new Row("", this.pan2, n++));
                this.box.add(new Row("", this.mix, n++));
                break;
            }
            case 5: {
                this.activeNibbles = new int[]{72, 57, 58, 60, 61, 62, 64, 65, 70};
                this.d1.updateNibble("Delay (ms)", 119);
                this.tempo1.updateNibble("Tempo", 21);
                this.width.updateNibble("Width (%)", 116);
                this.fb1.updateNibble("Feedback (%)", 116);
                this.hicut.updateNibble("HiCut (Hz)", 22);
                this.locut.updateNibble("LoCut (Hz)", 24);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("", this.d1, n++));
                this.box.add(new Row("", this.tempo1, n++));
                this.box.add(new Row("", this.width, n++));
                this.box.add(new Row("", this.fb1, n++));
                this.box.add(new Row("", this.hicut, n++));
                this.box.add(new Row("", this.locut, n++));
                this.box.add(new Row("", this.mix, n++));
            }
        }
        while (n < 14) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 5;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
