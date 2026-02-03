/*
 * Decompiled with CFR 0.152.
 */
package nova;

import nova.Block;
import nova.SystemDump;

public class Routing
extends Block {
    public Routing(SystemDump systemDump) {
        super("Routing", systemDump);
        this.rowspan = 2;
        this.activeNibbles = new int[]{1, 2, 51};
        for (int i = 0; i < this.activeNibbles.length; ++i) {
            this.nibbles.add(this.system.getNibble(this.activeNibbles[i]));
        }
    }

    void refresh() {
    }
}
