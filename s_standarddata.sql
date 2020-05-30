/*
Navicat MySQL Data Transfer

Source Server         : ocean
Source Server Version : 50505
Source Host           : 127.0.0.1:3306
Source Database       : autotest

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2020-05-30 14:04:11
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for s_standarddata
-- ----------------------------
DROP TABLE IF EXISTS `s_standarddata`;
CREATE TABLE `s_standarddata` (
  `vTestID` varchar(50) DEFAULT NULL COMMENT '测试编号',
  `vTitularValue` double(11,0) DEFAULT NULL COMMENT '温度名义值',
  `vStandardT` double DEFAULT NULL COMMENT '标准温度',
  `vStandardC` double DEFAULT NULL COMMENT '标准电导率',
  `vMeasureTime` datetime DEFAULT NULL COMMENT '测量时间',
  `vAddTime` datetime DEFAULT NULL,
  `vUpdateTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
