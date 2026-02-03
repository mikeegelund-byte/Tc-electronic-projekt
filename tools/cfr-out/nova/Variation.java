/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.Component;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Point;
import java.awt.event.ActionListener;
import java.awt.event.MouseEvent;
import java.io.File;
import java.io.IOException;
import java.util.List;
import java.util.Vector;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JPanel;
import javax.swing.JRootPane;
import nova.Block;
import nova.Comp;
import nova.Constants;
import nova.CopyBlock;
import nova.Delay;
import nova.DriveBoost;
import nova.EQGate;
import nova.Global;
import nova.Modulation;
import nova.Nibble;
import nova.NovaManager;
import nova.Patch;
import nova.Pitch;
import nova.Reverb;
import nova.VarPopup;

public class Variation
extends Patch
implements ActionListener {
    private static final int CMP = 0;
    private static final int DRV = 1;
    private static final int MOD = 2;
    private static final int DLY = 3;
    private static final int REV = 4;
    private static final int EQ = 5;
    private static final int PIT = 6;
    private static final String[] VARNAME = new String[]{"COMP", "DRIVE/BOOST", "MODULATION", "DELAY", "REVERB", "EQ/GATE", "PITCH"};
    private int numcode;
    private int blockType;
    private String variationName;
    private int n;
    JRootPane pane;
    JPanel titlePane;
    Global global;
    JButton closeButton = new JButton("Close");
    JButton saveButton = new JButton("Save SysEx");
    JButton saveXML = new JButton("Export XML");
    JButton saveHTML = new JButton("Export HTML");
    JComboBox send = new JComboBox();

    public Variation(NovaManager novaManager, byte[] byArray) {
        super(novaManager, byArray, true);
        CopyBlock copyBlock;
        this.refresh();
        this.setName(this.variationName);
        this.blocks = new Vector();
        this.global = new Global(this);
        DriveBoost driveBoost = new DriveBoost(this);
        Comp comp = new Comp(this);
        EQGate eQGate = new EQGate(this);
        Modulation modulation = new Modulation(this);
        Pitch pitch = new Pitch(this);
        Delay delay = new Delay(this);
        Reverb reverb = new Reverb(this);
        this.blocks.add(comp);
        this.blocks.add(driveBoost);
        this.blocks.add(modulation);
        this.blocks.add(delay);
        this.blocks.add(reverb);
        this.blocks.add(eQGate);
        this.blocks.add(pitch);
        this.blocks.add(this.global);
        this.closeButton.addActionListener(this);
        this.closeButton.setActionCommand("close");
        this.saveButton.addActionListener(this);
        this.saveButton.setActionCommand("save");
        this.saveXML.addActionListener(this);
        this.saveXML.setActionCommand("saveXML");
        this.saveHTML.addActionListener(this);
        this.saveHTML.setActionCommand("saveHTML");
        this.refreshNovasList();
        this.send.setActionCommand("send");
        this.send.setEnabled(false);
        switch (this.blockType) {
            case 0: {
                copyBlock = comp;
                break;
            }
            case 1: {
                copyBlock = driveBoost;
                break;
            }
            case 2: {
                copyBlock = modulation;
                break;
            }
            case 3: {
                copyBlock = delay;
                break;
            }
            case 4: {
                copyBlock = reverb;
                break;
            }
            case 5: {
                copyBlock = eQGate;
                break;
            }
            case 6: {
                copyBlock = pitch;
                break;
            }
            default: {
                copyBlock = null;
            }
        }
        JPanel jPanel = new JPanel();
        jPanel.setOpaque(false);
        jPanel.add(this.closeButton);
        jPanel.add(this.saveButton);
        jPanel.add(this.saveXML);
        jPanel.add(this.saveHTML);
        jPanel.add(this.send);
        this.setLayout(new GridBagLayout());
        GridBagConstraints gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.weighty = 0.0;
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 0;
        gridBagConstraints.fill = 2;
        gridBagConstraints.anchor = 19;
        this.add((Component)this.getTitlePane(), gridBagConstraints);
        gridBagConstraints.weighty = 0.0;
        gridBagConstraints.weightx = 0.0;
        ++gridBagConstraints.gridy;
        gridBagConstraints.fill = 0;
        gridBagConstraints.anchor = 19;
        this.add((Component)copyBlock, gridBagConstraints);
        ++gridBagConstraints.gridy;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.weighty = 1.0;
        gridBagConstraints.fill = 2;
        gridBagConstraints.anchor = 20;
        this.add((Component)jPanel, gridBagConstraints);
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
    }

    int getNumber() {
        return this.n;
    }

    void setNumber(int n) {
        this.n = n;
        this.numcode = 90 + this.blockType * 4 + this.n - 1;
        this.refreshVariationName();
    }

    void refreshVariationName() {
        this.variationName = VARNAME[this.blockType] + ": Variation #" + this.n;
        this.setName(this.variationName);
    }

    void refreshTitle() {
        this.refreshVariationName();
        this.global.refreshTitle();
        this.parent.refreshTabName(this.variationName);
    }

    void refresh() {
        super.refresh();
        this.numcode = this.getPresetCode();
        this.blockType = (this.numcode - 90) / 4;
        this.n = (this.numcode - 90) % 4 + 1;
        this.refreshVariationName();
    }

    void refreshNovasList() {
        this.send.removeActionListener(this);
        this.send.removeAllItems();
        this.send.addItem("Send to Nova");
        List list = this.parent.getNovas();
        if (list != null) {
            for (int i = 0; i < list.size(); ++i) {
                this.send.addItem(list.get(i));
            }
        }
        this.send.addActionListener(this);
    }

    String getVariationName() {
        return this.variationName;
    }

    void saveValues() {
        this.data[8] = (byte)this.numcode;
        for (int i = 0; i < this.nibbles.size(); ++i) {
            Nibble nibble = this.getNibble(i);
            nibble.save(this.data);
        }
        this.data[518] = (byte)this.calculateChecksum();
    }

    String toHTML() {
        String string = "";
        string = string + "<HTML>\n";
        string = string + "  <BODY>\n";
        string = string + "    <H1>" + this.variationName + "</H1>\n";
        string = string + "    <TABLE border=\"1\" cellpadding=\"10\">\n";
        string = string + "      <TR>\n";
        Block block = (Block)this.blocks.elementAt(this.blockType);
        string = string + block.toHTML();
        string = string + "      </TR>\n";
        string = string + "    </TABLE>\n";
        string = string + "  </BODY>\n";
        string = string + "</HTML>\n";
        return string;
    }

    public String toXML() {
        String string = "";
        string = string + "<variation vendorId=\"00201F\" vendor=\"TC Electronic\" ";
        string = string + "modelId=\"63\" model=\"Nova System\" ";
        string = string + " n=\"" + this.numcode;
        string = string + "\" block=\"" + this.blockType;
        string = string + "\" blockName=\"" + VARNAME[this.blockType];
        string = string + "\" variation=\"" + this.n;
        string = string + "\">\n";
        Block block = (Block)this.blocks.elementAt(this.blockType);
        string = string + block.toXML();
        string = string + "</variation>";
        return string;
    }

    public void mouseClicked(MouseEvent mouseEvent) {
        Component component = (Component)mouseEvent.getSource();
        Point point = component.getLocationOnScreen();
        int n = mouseEvent.getX() + point.x;
        int n2 = mouseEvent.getY() + point.y;
        if (n < 0) {
            n = 0;
        }
        if (n2 < 0) {
            n2 = 0;
        }
        VarPopup varPopup = new VarPopup(this.parent, this, n, n2);
        varPopup.pack();
        this.parent.setPopup(varPopup);
    }

    public static void main(String[] stringArray) {
        if (stringArray.length != 1) {
            System.out.println("Syntax : java nova.Variation <Sysex inputFile>");
        } else {
            File file = new File(stringArray[0]);
            try {
                byte[] byArray = Variation.getBytesFromFile(file);
                NovaManager novaManager = new NovaManager();
                novaManager.add(new Variation(novaManager, byArray));
            }
            catch (IOException iOException) {
                System.err.println("File not found : " + stringArray[1]);
            }
        }
    }
}
