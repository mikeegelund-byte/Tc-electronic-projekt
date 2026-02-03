/*
 * Decompiled with CFR 0.152.
 */
package nova;

import nova.Block;
import nova.SystemDump;

public class MidiCC
extends Block {
    public MidiCC(SystemDump systemDump) {
        super("MIDI CC", systemDump);
        this.rowspan = 2;
        this.activeNibbles = new int[]{8, 10, 9, 14, 16, 17, 11, 15, 12, 13, 18};
        for (int i = 0; i < this.activeNibbles.length; ++i) {
            this.nibbles.add(this.system.getNibble(this.activeNibbles[i]));
        }
    }

    void refresh() {
    }
}
