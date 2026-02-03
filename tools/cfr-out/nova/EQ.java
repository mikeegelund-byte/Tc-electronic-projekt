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

public class EQ
extends CopyBlock {
    Nibble f1;
    Nibble g1;
    Nibble w1;
    Nibble f2;
    Nibble g2;
    Nibble w2;
    Nibble f3;
    Nibble g3;
    Nibble w3;
    Nibble onoff;

    public EQ(Patch patch) {
        super("EQ", patch, true);
        try {
            this.onoff = this.parent.getNibble(93);
            this.onoff.updateNibble("On", 5);
            this.nibbles.add(this.onoff);
            this.f1 = this.parent.getNibble(94);
            this.f1.updateNibble("Freq1 (Hz)", 13);
            this.nibbles.add(this.f1);
            this.g1 = this.parent.getNibble(95);
            this.g1.updateNibble("Gain1 (dB)", 110);
            this.nibbles.add(this.g1);
            this.w1 = this.parent.getNibble(96);
            this.w1.updateNibble("Width1 (oct)", 14);
            this.nibbles.add(this.w1);
            this.f2 = this.parent.getNibble(97);
            this.f2.updateNibble("Freq2 (Hz)", 13);
            this.nibbles.add(this.f2);
            this.g2 = this.parent.getNibble(98);
            this.g2.updateNibble("Gain2 (dB)", 110);
            this.nibbles.add(this.g2);
            this.w2 = this.parent.getNibble(99);
            this.w2.updateNibble("Width2 (oct)", 14);
            this.nibbles.add(this.w2);
            this.f3 = this.parent.getNibble(100);
            this.f3.updateNibble("Freq3 (Hz)", 13);
            this.nibbles.add(this.f3);
            this.g3 = this.parent.getNibble(101);
            this.g3.updateNibble("Gain3 (dB)", 110);
            this.nibbles.add(this.g3);
            this.w3 = this.parent.getNibble(102);
            this.w3.updateNibble("Width3 (oct)", 14);
            this.nibbles.add(this.w3);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Drive Block");
        }
        this.refresh();
    }

    void refresh() {
        this.box.removeAll();
        int n = 0;
        this.activeNibbles = new int[]{93, 94, 95, 96, 97, 98, 99, 100, 101, 102};
        this.box.add(new Row("EQ", (Nibble)this.nibbles.elementAt(0), n++));
        this.box.add(new Row("F1", (Nibble)this.nibbles.elementAt(1), n++));
        this.box.add(new Row("G1", (Nibble)this.nibbles.elementAt(2), n++));
        this.box.add(new Row("W1", (Nibble)this.nibbles.elementAt(3), n++));
        this.box.add(new EmptyRow(false, n++));
        this.box.add(new Row("F2", (Nibble)this.nibbles.elementAt(4), n++));
        this.box.add(new Row("G2", (Nibble)this.nibbles.elementAt(5), n++));
        this.box.add(new Row("W2", (Nibble)this.nibbles.elementAt(6), n++));
        this.box.add(new EmptyRow(false, n++));
        this.box.add(new Row("F3", (Nibble)this.nibbles.elementAt(7), n++));
        this.box.add(new Row("G3", (Nibble)this.nibbles.elementAt(8), n++));
        this.box.add(new Row("W3", (Nibble)this.nibbles.elementAt(9), n++));
        while (n < 14) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 7;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
