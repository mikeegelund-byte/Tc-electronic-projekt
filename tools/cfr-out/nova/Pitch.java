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

public class Pitch
extends CopyBlock {
    Nibble type;
    Nibble v1;
    Nibble v2;
    Nibble p1;
    Nibble p2;
    Nibble d1;
    Nibble d2;
    Nibble fb1;
    Nibble fb2;
    Nibble l1;
    Nibble l2;
    Nibble pitch;
    Nibble range;
    Nibble mix;
    Nibble onoff;
    Nibble direction;
    Nibble key;
    Nibble scale;

    public Pitch(Patch patch) {
        super("Pitch", patch, true);
        this.activeNibbles = new int[]{120, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117};
        try {
            for (int i = 0; i < this.activeNibbles.length; ++i) {
                this.nibbles.add(this.parent.getNibble(this.activeNibbles[i]));
            }
            this.type = this.parent.getNibble(105);
            this.v1 = this.parent.getNibble(106);
            this.v2 = this.parent.getNibble(107);
            this.p1 = this.parent.getNibble(108);
            this.p2 = this.parent.getNibble(109);
            this.d1 = this.parent.getNibble(110);
            this.d2 = this.parent.getNibble(111);
            this.fb1 = this.parent.getNibble(112);
            this.fb2 = this.parent.getNibble(113);
            this.l1 = this.parent.getNibble(114);
            this.l2 = this.parent.getNibble(115);
            this.range = this.parent.getNibble(116);
            this.mix = this.parent.getNibble(117);
            this.onoff = this.parent.getNibble(120);
            this.type.updateNibble("Type", 55);
            this.onoff.updateNibble("On", 2);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Pitch Block");
        }
        this.direction = this.l2;
        this.key = this.fb1;
        this.scale = this.fb2;
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
        this.box.add(new Row("Pitch", this.onoff, n++));
        this.box.add(new Row("Pitch", this.type, n++));
        switch (this.type.getValue()) {
            case 0: {
                this.activeNibbles = new int[]{120, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 117};
                this.v1.updateNibble("Voice 1 (cents)", 127);
                this.v2.updateNibble("Voice 2 (cents)", 127);
                this.p1.updateNibble("Pan 1", 107);
                this.p2.updateNibble("Pan 2", 107);
                this.d1.updateNibble("Delay 1 (ms)", 126);
                this.d2.updateNibble("Delay 2 (ms)", 126);
                this.fb1.updateNibble("Feedback 1 (%)", 116);
                this.fb2.updateNibble("Feedback 2 (%)", 116);
                this.l1.updateNibble("Level 1 (dB)", 92);
                this.l2.updateNibble("Level 2 (dB)", 92);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("Pitch", this.v1, n++));
                this.box.add(new Row("Pitch", this.v2, n++));
                this.box.add(new Row("Pitch", this.p1, n++));
                this.box.add(new Row("Pitch", this.p2, n++));
                this.box.add(new Row("Pitch", this.d1, n++));
                this.box.add(new Row("Pitch", this.d2, n++));
                this.box.add(new Row("Pitch", this.fb1, n++));
                this.box.add(new Row("Pitch", this.fb2, n++));
                this.box.add(new Row("Pitch", this.l1, n++));
                this.box.add(new Row("Pitch", this.l2, n++));
                this.box.add(new Row("Pitch", this.mix, n++));
                break;
            }
            case 1: {
                this.activeNibbles = new int[]{120, 105, 115, 116, 117};
                this.direction.updateNibble("Direction", 44);
                this.range.updateNibble("Range (oct)", 120);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("Pitch", this.direction, n++));
                this.box.add(new Row("Pitch", this.range, n++));
                this.box.add(new Row("Pitch", this.mix, n++));
                break;
            }
            case 2: {
                this.activeNibbles = new int[]{120, 105, 114, 115, 116};
                this.pitch = this.l1;
                this.pitch.updateNibble("Pitch (%)", 116);
                this.direction.updateNibble("Direction", 44);
                this.range.updateNibble("Range (oct)", 120);
                this.direction.updateNibble("Direction", 44);
                this.box.add(new Row("Pitch", this.pitch, n++));
                this.box.add(new Row("Pitch", this.direction, n++));
                this.box.add(new Row("Pitch", this.range, n++));
                break;
            }
            case 3: {
                this.activeNibbles = new int[]{120, 105, 106, 107, 110, 111, 117};
                this.v1.updateNibble("Voice 1 (cents)", 103);
                this.v2.updateNibble("Voice 2 (cents)", 103);
                this.d1.updateNibble("Delay 1 (ms)", 128);
                this.d2.updateNibble("Delay 2 (ms)", 128);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("Pitch", this.v1, n++));
                this.box.add(new Row("Pitch", this.v2, n++));
                this.box.add(new Row("Pitch", this.d1, n++));
                this.box.add(new Row("Pitch", this.d2, n++));
                this.box.add(new Row("Pitch", this.mix, n++));
                break;
            }
            case 4: {
                this.activeNibbles = new int[]{120, 105, 112, 113, 106, 107, 114, 115, 108, 109, 110, 111, 117};
                this.key.updateNibble("Key", 40);
                this.scale.updateNibble("Scale", 41);
                this.v1.updateNibble("Voice 1 (degree)", 42);
                this.v2.updateNibble("Voice 2 (degree)", 42);
                this.l1.updateNibble("Level 1 (dB)", 92);
                this.l2.updateNibble("Level 2 (dB)", 92);
                this.p1.updateNibble("Pan 1", 107);
                this.p2.updateNibble("Pan 2", 107);
                this.d1.updateNibble("Delay 1 (ms)", 126);
                this.d2.updateNibble("Delay 2 (ms)", 126);
                this.mix.updateNibble("Mix (%)", 116);
                this.box.add(new Row("Pitch", this.key, n++));
                this.box.add(new Row("Pitch", this.scale, n++));
                this.box.add(new Row("Pitch", this.v1, n++));
                this.box.add(new Row("Pitch", this.v2, n++));
                this.box.add(new Row("Pitch", this.l1, n++));
                this.box.add(new Row("Pitch", this.l2, n++));
                this.box.add(new Row("Pitch", this.p1, n++));
                this.box.add(new Row("Pitch", this.p2, n++));
                this.box.add(new Row("Pitch", this.d1, n++));
                this.box.add(new Row("Pitch", this.d2, n++));
                this.box.add(new Row("Pitch", this.mix, n++));
            }
        }
        while (n < 14) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 4;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
