/*
 * Decompiled with CFR 0.152.
 */
package nova;

import javax.swing.Box;
import nova.CopyBlock;
import nova.Nibble;
import nova.Patch;
import nova.Row;

public class Reverb
extends CopyBlock {
    Nibble type;
    Nibble decay;
    Nibble preDly;
    Nibble shape;
    Nibble size;
    Nibble hicolor;
    Nibble hilvl;
    Nibble locolor;
    Nibble lolvl;
    Nibble roomlvl;
    Nibble revlvl;
    Nibble diffuse;
    Nibble mix;
    Nibble onoff;

    public Reverb(Patch patch) {
        super("Reverb", patch, true);
        try {
            this.type = this.parent.getNibble(73);
            this.type.updateNibble("Type", 53);
            this.nibbles.add(this.type);
            this.decay = this.parent.getNibble(74);
            this.decay.updateNibble("Decay (s)", 91);
            this.nibbles.add(this.decay);
            this.preDly = this.parent.getNibble(75);
            this.preDly.updateNibble("PreDelay (ms)", 116);
            this.nibbles.add(this.preDly);
            this.shape = this.parent.getNibble(76);
            this.shape.updateNibble("Shape", 32);
            this.nibbles.add(this.shape);
            this.size = this.parent.getNibble(77);
            this.size.updateNibble("Size", 33);
            this.nibbles.add(this.size);
            this.hicolor = this.parent.getNibble(78);
            this.hicolor.updateNibble("Hi Color", 30);
            this.nibbles.add(this.hicolor);
            this.hilvl = this.parent.getNibble(79);
            this.hilvl.updateNibble("Hi Fac", 109);
            this.nibbles.add(this.hilvl);
            this.locolor = this.parent.getNibble(80);
            this.locolor.updateNibble("Lo Color", 31);
            this.nibbles.add(this.locolor);
            this.lolvl = this.parent.getNibble(81);
            this.lolvl.updateNibble("Lo Fac", 109);
            this.nibbles.add(this.lolvl);
            this.roomlvl = this.parent.getNibble(82);
            this.roomlvl.updateNibble("Room Lvl (dB)", 132);
            this.nibbles.add(this.roomlvl);
            this.revlvl = this.parent.getNibble(83);
            this.revlvl.updateNibble("Rev Lvl (dB)", 132);
            this.nibbles.add(this.revlvl);
            this.diffuse = this.parent.getNibble(84);
            this.diffuse.updateNibble("Diffuse", 109);
            this.nibbles.add(this.diffuse);
            this.mix = this.parent.getNibble(85);
            this.mix.updateNibble("Mix (%)", 116);
            this.nibbles.add(this.mix);
            this.onoff = this.parent.getNibble(88);
            this.onoff.updateNibble("On", 2);
            this.nibbles.add(this.onoff);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Drive Block");
        }
        this.refresh();
    }

    void refresh() {
        this.box.removeAll();
        int n = 0;
        this.activeNibbles = new int[]{88, 73, 74, 75, 85, 77, 78, 79, 80, 81, 82, 83, 84, 76};
        Row row = new Row("Reverb", this.onoff, n++);
        this.box.add(row);
        row = new Row("Reverb", this.type, n++);
        this.box.add(row);
        row = new Row("Reverb", this.decay, n++);
        this.box.add(row);
        row = new Row("Reverb", this.preDly, n++);
        this.box.add(row);
        row = new Row("Reverb", this.mix, n++);
        this.box.add(row);
        row = new Row("Reverb", this.size, n++);
        this.box.add(row);
        row = new Row("Reverb", this.hicolor, n++);
        this.box.add(row);
        row = new Row("Reverb", this.hilvl, n++);
        this.box.add(row);
        row = new Row("Reverb", this.locolor, n++);
        this.box.add(row);
        row = new Row("Reverb", this.lolvl, n++);
        this.box.add(row);
        row = new Row("Reverb", this.roomlvl, n++);
        this.box.add(row);
        row = new Row("Reverb", this.revlvl, n++);
        this.box.add(row);
        row = new Row("Reverb", this.diffuse, n++);
        this.box.add(row);
        row = new Row("Reverb", this.shape, n++);
        this.box.add(row);
    }

    int getBlockType(Box box) {
        return 6;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
