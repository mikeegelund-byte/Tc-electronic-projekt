/*
 * Decompiled with CFR 0.152.
 */
package nova;

import nova.Block;
import nova.SystemDump;

public class Levels
extends Block {
    public Levels(SystemDump systemDump) {
        super("Levels", systemDump);
        this.rowspan = 3;
        this.activeNibbles = new int[]{49, 50, 36, 45, 43, 46, 44, 47, 48, 37, 42, 40};
        for (int i = 0; i < this.activeNibbles.length; ++i) {
            this.nibbles.add(this.system.getNibble(this.activeNibbles[i]));
        }
    }

    void refresh() {
    }
}
