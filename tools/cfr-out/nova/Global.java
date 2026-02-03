/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.Color;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.Font;
import javax.swing.Box;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import nova.Constants;
import nova.CopyBlock;
import nova.Nibble;
import nova.Patch;
import nova.Row;
import nova.Variation;

public class Global
extends CopyBlock {
    String name = "";
    int bankN;
    int presN;
    int numcode;
    JLabel bankLbl;
    JLabel presLbl;
    JTextField bankTf;
    JTextField presTf;
    JTextField nameTf;
    JLabel titleLabel;

    public Global(Patch patch) {
        super("Global settings", patch, false);
        this.activeNibbles = new int[]{1, 2, 3, 4, 5, 6, 7, 8};
        if (this.parent instanceof Variation) {
            this.name = ((Variation)this.parent).getVariationName();
        } else {
            this.name = this.parent.getOriginalName();
            this.bankN = this.parent.getOriginalBankNumber();
            this.presN = this.parent.getOriginalPresetNumber();
            this.numcode = this.parent.getPresetCode();
        }
        this.titleLabel = new JLabel("", 0);
        this.titleLabel.setFont(new Font("Verdana", 1, 20));
        this.titleLabel.setForeground(Color.white);
        this.titleLabel.setPreferredSize(new Dimension(300, 60));
        if (this.parent.parent.isEditable()) {
            this.titleLabel.addMouseListener(this.parent);
        }
        JPanel jPanel = this.parent.getTitlePane();
        jPanel.removeAll();
        jPanel.add((Component)this.titleLabel, "Center");
        try {
            Nibble nibble = this.parent.getNibble(1);
            nibble.updateNibble("Tempo", 123);
            this.nibbles.add(nibble);
            Nibble nibble2 = this.parent.getNibble(2);
            nibble2.updateNibble("Routing", 3);
            this.nibbles.add(nibble2);
            Nibble nibble3 = this.parent.getNibble(3);
            nibble3.updateNibble("LvlOut L (dB)", 102);
            this.nibbles.add(nibble3);
            Nibble nibble4 = this.parent.getNibble(4);
            nibble4.updateNibble("LvlOut R (dB)", 102);
            this.nibbles.add(nibble4);
            Nibble nibble5 = this.parent.getNibble(5);
            nibble5.updateNibble("Map Param", 4);
            this.nibbles.add(nibble5);
            Nibble nibble6 = this.parent.getNibble(6);
            nibble6.updateNibble("Map Min (%)", 116);
            this.nibbles.add(nibble6);
            Nibble nibble7 = this.parent.getNibble(7);
            nibble7.updateNibble("Map Med (%)", 116);
            this.nibbles.add(nibble7);
            Nibble nibble8 = this.parent.getNibble(8);
            nibble8.updateNibble("Map Max (%)", 116);
            this.nibbles.add(nibble8);
        }
        catch (Exception exception) {
            exception.printStackTrace();
            System.err.println("Not enough Nibbles to load Global Block");
        }
        this.refresh();
    }

    void refreshTitle() {
        String string;
        if (this.parent instanceof Variation) {
            string = this.name = ((Variation)this.parent).getVariationName();
        } else {
            this.numcode = this.parent.getPresetCode();
            this.name = this.parent.getOriginalName();
            string = "Preset #" + this.numcode + " [" + Constants.PRESET[this.numcode] + "]  \"" + this.name + "\"";
        }
        this.titleLabel.setText(string);
    }

    void refresh() {
        this.refreshTitle();
        Nibble nibble = this.parent.getNibble(5);
        nibble.setValueList(this.parent.getExpressionPedalValueList());
        nibble.setTypeList(this.parent.exp_pedal_valueList);
        this.box.removeAll();
        this.box.add(new Row("Global settings", null, 0));
        this.activeNibbles = new int[]{1, 2, 3, 4, 5, 6, 7, 8};
        for (int i = 0; i < this.nibbles.size(); ++i) {
            this.box.add(new Row("Global settings", (Nibble)this.nibbles.elementAt(i), i + 1));
        }
    }

    String getPresetName() {
        return this.nameTf.getText();
    }

    int getBankNumber() {
        int n = 0;
        try {
            n = Integer.parseInt(this.bankTf.getText());
        }
        catch (Exception exception) {
            exception.printStackTrace();
        }
        return n;
    }

    int getPresetNumber() {
        int n = 0;
        try {
            n = Integer.parseInt(this.presTf.getText());
        }
        catch (Exception exception) {
            exception.printStackTrace();
        }
        return n;
    }

    int getBlockType(Box box) {
        return 30;
    }

    int[] getActiveNibbles(Box box) {
        return this.activeNibbles;
    }

    void setActiveNibbles(Box box, int[] nArray) {
        this.activeNibbles = nArray;
    }
}
