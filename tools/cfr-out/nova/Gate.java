/*
 * Decompiled with CFR 0.152.
 */
package nova;

import javax.swing.Box;
import javax.swing.JLabel;
import javax.swing.JTextField;
import nova.CopyBlock;
import nova.EmptyRow;
import nova.Nibble;
import nova.Patch;
import nova.Row;

public class Gate
extends CopyBlock {
    String name = "";
    int bankN;
    int presN;
    JLabel bankLbl;
    JLabel presLbl;
    JTextField bankTf;
    JTextField presTf;
    JTextField nameTf;
    Nibble type;
    Nibble threshold;
    Nibble damp;
    Nibble release;
    Nibble onoff;

    public Gate(Patch patch) {
        super("Noise Gate", patch, true);
        try {
            this.type = this.parent.getNibble(89);
            this.type.updateNibble("Type", 56);
            this.nibbles.add(this.type);
            this.threshold = this.parent.getNibble(90);
            this.threshold.updateNibble("Threshold (dB)", 105);
            this.nibbles.add(this.threshold);
            this.damp = this.parent.getNibble(91);
            this.damp.updateNibble("Damp (dB)", 115);
            this.nibbles.add(this.damp);
            this.release = this.parent.getNibble(92);
            this.release.updateNibble("Speed (/s)", 125);
            this.nibbles.add(this.release);
            this.onoff = this.parent.getNibble(104);
            this.onoff.updateNibble("On", 5);
            this.nibbles.add(this.onoff);
        }
        catch (Exception exception) {
            System.err.println("Not enough Nibbles to load Gate Block");
        }
        this.refresh();
    }

    void refresh() {
        this.box.removeAll();
        int n = 0;
        this.activeNibbles = new int[]{104, 89, 90, 91, 92};
        this.box.add(new Row("Noise Gate", this.onoff, n++));
        this.box.add(new Row("Noise Gate", this.type, n++));
        this.box.add(new Row("Noise Gate", this.threshold, n++));
        this.box.add(new Row("Noise Gate", this.damp, n++));
        this.box.add(new Row("Noise Gate", this.release, n++));
        while (n < 9) {
            this.box.add(new EmptyRow(true, n++));
        }
    }

    int getBlockType(Box box) {
        return 8;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
